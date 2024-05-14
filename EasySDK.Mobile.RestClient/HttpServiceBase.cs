#nullable enable

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

	#endregion

	#region Private fields

	private readonly          IHttpClientFactory _httpClientFactory;
	private readonly          ITokenProvider     _tokenProvider;
	private readonly          Uri                _baseUri;
	private readonly          bool               _useGZip;
	private readonly          bool               _autoReauthorize;
	protected static readonly HttpMethod         HttpMethodPatch = new("PATCH");

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
		bool useGZip = false, 
		bool autoReauthorize = true
	)
	{
		_httpClientFactory    = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		Logger                = logger;
		_tokenProvider        = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		_baseUri              = baseUri;
		_useGZip              = useGZip;
		_autoReauthorize = autoReauthorize;
	}

	#endregion

	#region Protected methods

	protected async Task<IResponse<TResult>> PostJsonAsync<TResult>
	(
		string requestUrl,
		object? postModel,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => postModel != null ? CreateJsonContent(postModel) : new StringContent(string.Empty),
			(c, r, t) => c.PostAsync(requestUrl, r, t),
			c => CreateResponse(c, parse),
			useToken,
			cancellationToken
		);
	}

	protected async Task<IResponseList<TResult>> GetJsonListAsync<TResult>
	(
		string requestUrl,
		object? filter = null,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponseList<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => CreateGetMessage(requestUrl, filter),
			(c, r, t) => c.SendAsync(r, t),
			(c) =>
			{
				var result = CreateResponse(c, parse);

				if (result?.Metadata is { } metadata && metadata.Value<int>("total") is { } total)
					result.TotalCount = total;

				return result;
			},
			useToken,
			cancellationToken
		);
	}

	protected async Task<IResponse<TResult>> GetJsonAsync<TResult>
	(
		string requestUrl,
		object? model = null,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => CreateGetMessage(requestUrl, model),
			(c, r, t) => c.SendAsync(r, t),
			c => CreateResponse(c, parse),
			useToken,
			cancellationToken
		);
	}

	protected async Task<IResponse<TResult>> PatchJsonAsync<TResult>
	(
		string requestUrl, 
		object? model,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => new HttpRequestMessage(HttpMethodPatch, requestUrl)
			{
				Content = model != null ? CreateJsonContent(model) : new StringContent(string.Empty)
			},
			(c, r, t) => c.SendAsync(r, t),
			c => CreateResponse(c, parse),
			useToken,
			cancellationToken
		);
	}

	protected Task<IResponse<TResult>?> PatchJsonAsync<TForm, TResult>
	(
		string requestUri,
		Expression<Func<TForm>> patch,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		var model = patch.ToJObject();

		return PatchJsonAsync(requestUri, model, useToken, parse, cancellationToken);
	}

	protected async Task<IResponse<TResult>?> DeleteJsonAsync<TResult>
	(
		string requestUrl,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => new StringContent(string.Empty),
			(c, _, t) => c.DeleteAsync(requestUrl, t),
			c => CreateResponse(c, parse),
			useToken,
			cancellationToken
		);
	}

	protected async Task<IResponse<TResult>?> PostFormAsync<TResult>
	(
		string requestUrl,
		HttpContent form,
		bool useToken = true,
		Func<JObject, JsonSerializer, HttpResponse<TResult>>? parse = null,
		CancellationToken cancellationToken = default
	)
	{
		return await ExecuteAsync
		(
			() => form,
			(c, r, t) => c.PostAsync(requestUrl, r, t),
			c => CreateResponse(c, parse),
			useToken,
			cancellationToken
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

		Logger.LogDebug("Create json for send: {0}", json);

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
	
	protected async Task<TResponse> ExecuteAsync<TResponse, TRequest>
	(
		Func<TRequest> requestFactory,
		Func<HttpClient, TRequest, CancellationToken, Task<HttpResponseMessage>> execute,
		Func<string, TResponse> responseFactory,
		bool useToken,
		CancellationToken cancellationToken
	)
		where TRequest : class, IDisposable
		where TResponse : HttpResponse, new()
	{

		HttpResponseMessage? response = null;

		try
		{
			using var client = CreateClient();

			var stopwatch = Stopwatch.StartNew();

			response = await GetResponse
			(
				client,
				useToken,
				requestFactory,
				execute,
				cancellationToken
			);

			if (response == null)
			{
				Logger.LogInformation("Response is null.");
				return null;
			}

			var content = await response.Content.ReadAsStringAsync();

			Logger.LogDebug("Response content: {0}", content);

			stopwatch.Stop();
			Logger.LogInformation
			(
				"Execute {0} '{1}' completed in time: {2}.",
				response.RequestMessage.Method,
				response.RequestMessage.RequestUri,
				stopwatch.Elapsed
			);

			Logger.LogDebug(content);

			if (!response.IsSuccessStatusCode)
				return CreateErrorResponse<TResponse>(response, content);

			var result = responseFactory(content);

			if (result?.HasError == true)
				LogErrorResponse(response, content);

			return result ?? default!;
		}
		finally
		{
			response?.Dispose();
		}
	}

	private async Task<HttpResponseMessage?> GetResponse<TRequest>
	(
		HttpClient client,
		bool useToken,
		Func<TRequest> requestFactory,
		Func<HttpClient, TRequest, CancellationToken, Task<HttpResponseMessage>> execute,
		CancellationToken cancelToken
	) where TRequest : class, IDisposable
	{
		var stopwatch = Stopwatch.StartNew();
		HttpResponseMessage? r = null;

		for (int i = 0; i < 2; i++)
		{
			if (useToken)
			{
				Logger.LogInformation("Try set token...");
				stopwatch.Restart();
				await SetToken(client);
				Logger.LogInformation("Set token for: {0} time.", stopwatch.Elapsed);
			}

			using var request = requestFactory();

			Logger.LogInformation("Try get response...");
			stopwatch.Restart();
			r = await execute(client, request, cancelToken);
			Logger.LogInformation("Get response for {0} time", stopwatch.Elapsed);

			if (r.IsSuccessStatusCode
			    || r.StatusCode != HttpStatusCode.Unauthorized
			    || !useToken)
			{
				Logger.LogInformation("Response is success.");
				return r;
			}

			if (i > 0 || !_autoReauthorize)
			{
				Logger.LogInformation("Response is failed.");
				return r;
			}

			var isUnauthorized = r.StatusCode == HttpStatusCode.Unauthorized;

			r.Dispose();

			if (isUnauthorized)
			{
				Logger.LogInformation("Try reset token.");
				stopwatch.Restart();
				await _tokenProvider.InvalidateToken();
				Logger.LogInformation("Reset token for {0} time.", stopwatch.Elapsed);
			}

			if (cancelToken.IsCancellationRequested)
				return r;
		}

		return r;
	}

	protected async Task SetToken(HttpClient client)
	{
		var token = await _tokenProvider.TryGetAuthTokenAsync();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}

	#endregion

	#region Prrivate methods

	private HttpRequestMessage CreateGetMessage(string requestUrl, object? model) => new()
	{
		Content = model != null ? CreateJsonContent(model) : null,
		Method = HttpMethod.Get,
		RequestUri = new Uri(requestUrl, UriKind.Relative)
	};

	#endregion
}