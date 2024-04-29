using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Maui
{
	public class IosHttpCLientFactory : IHttpClientFactory
	{
		private readonly NSUrlSessionHandler _handler = new();

		public HttpClient CreateHttpClient() => new(_handler);
	}
}
