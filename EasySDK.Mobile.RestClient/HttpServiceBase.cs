using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;
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

	protected virtual JsonSerializerSettings CreateJsonSettings() => new()
	{
		Culture = CultureInfo.CurrentUICulture,
		DateFormatString = "dd.MM.yyyy",
		DateParseHandling = DateParseHandling.DateTime,
		DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
	};

	protected StringContent CreateJsonContent<TModel>(TModel model, JsonSerializerSettings settings = null)
	{
		var json = JsonConvert.SerializeObject(model, settings);

		return new StringContent(json, Encoding.UTF8, MediaType);
	}

	protected void LogErrorResponse(HttpResponseMessage response, string content)
	{
		Logger.LogWarning("Response from {0}  error: {1}", response.RequestMessage.RequestUri, content);
	}

	protected HttpClient CreateClient(bool useToken = true)
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
		
		if (useToken)
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);

		return client;
	}

	protected async Task<IResponse<TResult>> PostJsonAsync<TResult>
	(
		string requestUrl, 
		object postModel,
		bool useToken = true)
	{
		var stopwatch = Stopwatch.StartNew();
		using var requestContent = postModel != null ? CreateJsonContent(postModel) : new StringContent(string.Empty);
		using var client = CreateClient(useToken);
		using var response = await client.PostAsync(requestUrl, requestContent);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute POST '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());

		if(result?.HasError == true)
			LogErrorResponse(response, content);

		return result;
	}

	protected async Task<IResponseList<TResult>> GetJsonListAsync<TResult>
	(
		string requestUrl,
		object filter = null,
		bool useToken = true
	)
	{
		var stopwatch = Stopwatch.StartNew();
		using var client = CreateClient(useToken);
		using var requestContent = filter != null ? CreateJsonContent(filter) : new StringContent(string.Empty);
		using var request = new HttpRequestMessage
		{
			Content = requestContent, 
			Method = HttpMethod.Get,
			RequestUri = new Uri(requestUrl, UriKind.Relative)
		};
		using var response = await client.SendAsync(request);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute GET '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponseList<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponseList<TResult>>(content, CreateJsonSettings());

		if(result?.HasError == true)
			LogErrorResponse(response, content);

		if (result?.Metadata is { } metadata && metadata.Value<int>("total") is { } total)
			result.TotalCount = total;

		return result;
	}

	protected async Task<IResponse<TResult>> GetJsonAsync<TResult>
	(
		string requestUrl,
		bool useToken = true
	)
	{
		var stopwatch = Stopwatch.StartNew();
		using var client = CreateClient(useToken);
		using var response = await client.GetAsync(requestUrl);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute GET '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());

		if(result?.HasError == true)
			LogErrorResponse(response, content);

		return result;
	}

	protected async Task<IResponse<TResult>> PatchJsonAsync<TResult>
	(
		string requestUrl, 
		object model,
		bool useToken = true
	)
	{
		var stopwatch = Stopwatch.StartNew();
		using var client = CreateClient(useToken);
		using var requestContent = model != null ? CreateJsonContent(model) : new StringContent(string.Empty);
		using var request = new HttpRequestMessage(HttpMethodPatch, requestUrl)
		{
			Content = requestContent
		};
		using var response = await client.SendAsync(request);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute PATCH '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);
		
		var result = JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
		
		if(result?.HasError == true)
			LogErrorResponse(response, content);

		return result;
	}

	protected async Task<IResponse<TResult>> DeleteJsonAsync<TResult>
	(
		string requestUrl,
		bool useToken = true
	)
	{
		var stopwatch = Stopwatch.StartNew();
		using var client = CreateClient(useToken);
		using var response = await client.DeleteAsync(requestUrl);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute DELETE '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());

		if(result?.HasError == true)
			LogErrorResponse(response, content);

		return result;
	}

	protected async Task<IResponse<TResult>> PostFormAsync<TResult>
	(
		string requestUrl,
		HttpContent form,
		bool useToken = true
	)
	{
		var stopwatch = Stopwatch.StartNew();
		using var client = CreateClient(useToken);
		using var response = await client.PostAsync(requestUrl, form);

		var content = await response.Content.ReadAsStringAsync();

		stopwatch.Stop();
		Logger.LogInformation("Execute POST '{0}' completed in time: {1}.", requestUrl, stopwatch.Elapsed);

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());

		if(result?.HasError == true)
			LogErrorResponse(response, content);

		return result;
	}

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

	#endregion
}