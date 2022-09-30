using System.Net;
using System.Net.Http;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Android.Services;

public class AndroidHttpClientFactory : IHttpClientFactory
{
	public HttpClient CreateHttpClient() => new(new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
		AllowAutoRedirect = true,
		AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
	});
}