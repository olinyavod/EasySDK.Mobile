using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.RestClient.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EasySDK.Mobile.RestClient;

public abstract class HttpServiceBase
{
	#region Constants

	protected const string MediaType = "application/json";

	#endregion

	#region Private fields

	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger _logger;
	private readonly ITokenProvider _tokenProvider;
	private readonly Uri _baseUri;
	protected static readonly HttpMethod HttpMethodPatch = new("PATCH");

	#endregion

	#region ctor

	protected HttpServiceBase
	(
		IHttpClientFactory httpClientFactory,
		ILogger logger,
		ITokenProvider tokenProvider,
		Uri baseUri
	)
	{
		_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		_logger = logger;
		_tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		_baseUri = baseUri;
	}

	#endregion

	#region Protected methods

	protected JsonSerializerSettings CreateJsonSettings() => new()
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
		_logger.LogWarning("Response from {0}  error: {1}", response.RequestMessage.RequestUri, content);
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
		using var requestContent = postModel != null ? CreateJsonContent(postModel) : new StringContent(string.Empty);
		using var client = CreateClient(useToken);
		using var response = await client.PostAsync(requestUrl, requestContent);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		return JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
	}

	protected async Task<IResponseList<TResult>> GetJsonListAsync<TResult>
	(
		string requestUrl,
		object filter = null,
		bool useToken = true
	)
	{
		using var client = CreateClient(useToken);
		using var requestContent = filter != null ? CreateJsonContent(filter) : new StringContent(string.Empty);
		using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl) {Content = requestContent};
		using var response = await client.SendAsync(request);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponseList<TResult>>(response, content);

		var result = JsonConvert.DeserializeObject<HttpResponseList<TResult>>(content, CreateJsonSettings());

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
		using var client = CreateClient(useToken);
		using var response = await client.GetAsync(requestUrl);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		return JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
	}

	protected async Task<IResponse<TResult>> PatchJsonAsync<TResult>
	(
		string requestUrl, 
		object model,
		bool useToken = true
	)
	{
		using var client = CreateClient(useToken);
		using var requestContent = model != null ? CreateJsonContent(model) : new StringContent(string.Empty);
		using var request = new HttpRequestMessage(HttpMethodPatch, requestUrl)
		{
			Content = requestContent
		};
		using var response = await client.SendAsync(request);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);
		
		return JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
	}

	protected async Task<IResponse<TResult>> DeleteJsonAsync<TResult>
	(
		string requestUrl,
		bool useToken = true
	)
	{
		using var client = CreateClient(useToken);
		using var response = await client.DeleteAsync(requestUrl);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		return JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
	}

	protected async Task<IResponse<TResult>> PostFormAsync<TResult>
	(
		string requestUrl,
		HttpContent form,
		bool useToken = true
	)
	{
		using var client = CreateClient(useToken);
		using var response = await client.PostAsync(requestUrl, form);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return CreateErrorResponse<HttpResponse<TResult>>(response, content);

		return JsonConvert.DeserializeObject<HttpResponse<TResult>>(content, CreateJsonSettings());
	}

	protected TResponse CreateErrorResponse<TResponse>
	(
		HttpResponseMessage response,
		string content
	) where TResponse : HttpResponse, new()
	{
		LogErrorResponse(response, content);

		var errorCode = (int) response.StatusCode;

		return new TResponse()
		{
			ErrorMessage = content,
			ErrorCode = errorCode
		};
	}

	#endregion
}