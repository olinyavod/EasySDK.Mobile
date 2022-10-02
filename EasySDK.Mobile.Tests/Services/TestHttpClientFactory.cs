using System.Net;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Tests.Services;

public class TestHttpClientFactory : IHttpClientFactory
{
	public HttpClient CreateHttpClient() => new(new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
		AllowAutoRedirect = true,
		AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
	});
}