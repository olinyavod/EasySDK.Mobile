﻿#nullable enable

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.RestClient.Extensions;
using EasySDK.Mobile.RestClient.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.RestClient;

public abstract class HttpServiceBase
{
	#region Constants

	protected const string MediaType = "application/json";

	private static readonly object _sync = new();

	#endregion

	#region Private fields

	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ITokenProvider _tokenProvider;
	private readonly Uri _baseUri;
	private readonly bool _useGZip;
	protected static readonly HttpMethod HttpMethodPatch = new("PATCH");

	#endregion

	#region Properties

	protected ILogger Logger { get; }

	#endregion

	#region ctor

	protected HttpServiceBase
	(
		IHttpClientFactory httpClientFactory,
		ILogger logger,
		ITokenProvider tokenProvider,
		Uri baseUri,
		bool useGZip = false
	)
	{
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		Logger = logger;
		_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		_baseUri = baseUri;
		_useGZip = useGZip;
	}

	#endregion

	#region Protected methods

	protected async Task<IResponse<TResult>?> PostJsonAsync<TResult>
	(
		string requestUrl,
		object? postModel,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => postModel != null ? CreateJsonContent(postModel) : new StringContent(string.Empty),
			(c, r) => c.PostAsync(requestUrl, r),
			c => CreateResponse(c, parse),
			useToken
		);
	}

	protected async Task<IResponseList<TResult>?> GetJsonListAsync<TResult>
	(
		string requestUrl,
		object? filter = null,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponseList<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => CreateGetMessage(requestUrl, filter),
			(c, r) => c.SendAsync(r),
			(c) =>
			{
				var result = CreateResponse(c, parse);

				if (result?.Metadata is { } metadata && metadata.Value<int>("total") is { } total)
					result.TotalCount = total;

				return result;
			},
			useToken
		);
	}

	protected async Task<IResponse<TResult>?> GetJsonAsync<TResult>
	(
		string requestUrl,
		object? model = null,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => CreateGetMessage(requestUrl, model),
			(c, r) => c.SendAsync(r),
			c => CreateResponse(c, parse),
			useToken
		);
	}

	protected async Task<IResponse<TResult>?> PatchJsonAsync<TResult>
	(
		string requestUrl, 
		object? model,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => new HttpRequestMessage(HttpMethodPatch, requestUrl)
			{
				Content = model != null ? CreateJsonContent(model) : new StringContent(string.Empty)
			},
			(c, r) => c.SendAsync(r),
			c => CreateResponse(c, parse),
			useToken
		);
	}

	protected Task<IResponse<TResult>?> PatchJsonAsync<TForm, TResult>
	(
		string requestUri,
		Expression<Func<TForm>> patch,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		var model = patch.ToJObject();

		return PatchJsonAsync(requestUri, model, useToken, parse);
	}

	protected async Task<IResponse<TResult>?> DeleteJsonAsync<TResult>
	(
		string requestUrl,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => new StringContent(string.Empty),
			(c, _) => c.DeleteAsync(requestUrl),
			c => CreateResponse(c, parse),
			useToken
		);
	}

	protected async Task<IResponse<TResult>?> PostFormAsync<TResult>
	(
		string requestUrl,
		HttpContent form,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null
	)
	{
		return await ExecuteAsync
		(
			() => form,
			(c, r) => c.PostAsync(requestUrl, r),
			c => CreateResponse(c, parse),
			useToken
		);
	}

	protected virtual JsonSerializerSettings CreateJsonSettings() => new()
	{
		Culture = CultureInfo.CurrentUICulture,
		DateFormatString = "dd.MM.yyyy",
		DateParseHandling = DateParseHandling.DateTime,
		DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
	};

	protected TResponse CreateErrorResponse<TResponse>
	(
		HttpResponseMessage response,
		string content
	) where TResponse : HttpResponse, new()
	{
		LogErrorResponse(response, content);

		var json = JToken.Parse(content);
		var message = json.Value<string>("message") ?? content;

		var errorCode = (int) response.StatusCode;

		return new TResponse()
		{
			ErrorMessage = message,
			ErrorCode = errorCode
		};
	}

	protected async Task CopyStreamAsync(IProgressRequest progress, long length, Stream source, Stream destination, int bufferSize = 1024)
	{
		progress.RaiseProgressChanged(0, length);
		var buffer = new byte[bufferSize];
		var total = 0;

		while ((await source.ReadAsync(buffer, 0, buffer.Length)) is { } readCount and > 0)
		{
			await destination.WriteAsync(buffer, 0, readCount);
			total += readCount;
			progress.RaiseProgressChanged(total, length);
		}
	}

	protected StringContent CreateJsonContent<TModel>(TModel model, JsonSerializerSettings? settings = null)
	{
		var json = JsonConvert.SerializeObject(model, settings);

		return new StringContent(json, Encoding.UTF8, MediaType);
	}

	protected void LogErrorResponse(HttpResponseMessage response, string content)
	{
		Logger.LogWarning("Response from {0}  error: {1}", response.RequestMessage.RequestUri, content);
	}

	protected HttpClient CreateClient()
	{
		var client = _httpClientFactory.CreateHttpClient();

		client.BaseAddress = _baseUri;
		client.Timeout = TimeSpan.FromMinutes(10);
		client.MaxResponseContentBufferSize = 100 * 1024 * 1024;
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
		client.DefaultRequestHeaders.AcceptCharset.TryParseAdd("utf-8");
		client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(CultureInfo.CurrentUICulture.Name);

		if (_useGZip)
			client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");

		return client;
	}

	protected TResponse? CreateResponse<TResponse>(string content, Func<JObject, JsonSerializer, TResponse>? parse)
		where TResponse : IResponse
	{
		var jsonObject = JObject.Parse(content);
		var serializer = JsonSerializer.Create(CreateJsonSettings());
		var result = parse != null
			? parse(jsonObject, serializer)
			: jsonObject.ToObject<TResponse>(serializer);
		
		return result;
	}
	
	protected async Task<TResponse?> ExecuteAsync<TResponse, TRequest>
	(
		Func<TRequest> requestFactory,
		Func<HttpClient, TRequest, Task<HttpResponseMessage>> execute,
		Func<string, TResponse?> responseFactory,
		bool useToken
	)
		where TRequest : class, IDisposable
		where TResponse : HttpResponse, new()
	{

		HttpResponseMessage? response = null;
		try
		{
			using var client = CreateClient();

			async Task<HttpResponseMessage> GetResponse(HttpClient c, bool requestToken)
			{
				if (useToken)
					await SetToken(c, requestToken);

				using var request = requestFactory();
				var r = await execute(c, request);

				if (r.IsSuccessStatusCode
				    || r.StatusCode != HttpStatusCode.Unauthorized
				    || !requestToken
				    || !useToken)
					return r;

				r.Dispose();

				try
				{
					Monitor.Enter(_sync);
					await _tokenProvider.InvalidateToken();

					return await GetResponse(c, false);
				}
				finally
				{
					Monitor.Exit(_sync);
				}
			}

			var stopwatch = Stopwatch.StartNew();

			response = await GetResponse(client, true);

			var content = await response.Content.ReadAsStringAsync();

			stopwatch.Stop();
			Logger.LogInformation
			(
				"Execute {0} '{1}' completed in time: {2}.",
				response.RequestMessage.Method,
				response.RequestMessage.RequestUri,
				stopwatch.Elapsed
			);

			if (!response.IsSuccessStatusCode)
				return CreateErrorResponse<TResponse>(response, content);

			var result = responseFactory(content);

			if (result?.HasError == true)
				LogErrorResponse(response, content);

			return result;
		}
		finally
		{
			response?.Dispose();
		}
	}

	protected async Task SetToken(HttpClient client, bool locked)
	{
		try
		{
			if (locked)
				Monitor.Enter(_sync);

			var token = await _tokenProvider.TryGetAuthTokenAsync();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		finally
		{
			if (locked)
				Monitor.Exit(_sync);
		}
	}

	#endregion

	#region Prrivate methods

	private HttpRequestMessage CreateGetMessage(string requestUrl, object? model) => new()
	{
		Content = model != null ? CreateJsonContent(model) : new StringContent(string.Empty),
		Method = HttpMethod.Get,
		RequestUri = new Uri(requestUrl, UriKind.Relative)
	};

	#endregion
}