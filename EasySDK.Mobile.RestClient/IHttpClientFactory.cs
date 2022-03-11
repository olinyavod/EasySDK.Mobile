using System.Net.Http;

namespace EasySDK.Mobile.RestClient;

public interface IHttpClientFactory
{
	HttpClient CreateHttpClient();
}