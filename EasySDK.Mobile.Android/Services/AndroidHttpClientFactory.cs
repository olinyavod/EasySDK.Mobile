using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Android.Services;

public class AndroidHttpClientFactory : IHttpClientFactory, IDisposable
{
	private readonly Lazy<HttpClientHandler> _handler = new(() => new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
		AllowAutoRedirect = true, 
		SslProtocols = SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls12 | SslProtocols.Tls13,
		AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
	});

	public HttpClient CreateHttpClient() => new(_handler.Value, false);

	public void Dispose()
	{
		if(_handler.IsValueCreated)
			_handler.Value.Dispose();
	}
}