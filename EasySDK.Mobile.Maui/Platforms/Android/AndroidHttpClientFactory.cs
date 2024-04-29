using System.Net;
using System.Security.Authentication;
using EasySDK.Mobile.RestClient;
using Xamarin.Android.Net;

namespace EasySDK.Mobile.Maui
{
	public class AndroidHttpClientFactory : IHttpClientFactory
	{
		private readonly AndroidMessageHandler _handler = new()
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.GZip, 
			SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
		};
		
		public HttpClient CreateHttpClient() => new(_handler, false);
	}
}
