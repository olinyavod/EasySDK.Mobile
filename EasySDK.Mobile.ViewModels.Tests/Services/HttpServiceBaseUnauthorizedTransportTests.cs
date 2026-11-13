using System.Net;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.RestClient;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasySDK.Mobile.ViewModels.Tests.Services;

public class HttpServiceBaseUnauthorizedTransportTests
{
	[TestCase(HttpStatusCode.Unauthorized, "")]
	[TestCase(HttpStatusCode.Unauthorized, "<html>Unauthorized</html>")]
	[TestCase(HttpStatusCode.Unauthorized, "not JSON")]
	[TestCase(HttpStatusCode.OK, "{\"code\":401,\"errorMessage\":\"Expired\"}")]
	public async Task First_party_no_retry_response_retains_401(HttpStatusCode status, string body)
	{
		var handler = new ResponseHandler(status, body);
		var service = new TestService(new ClientFactory(handler));
		var response = await service.Call();
		Assert.That(response.ErrorCode, Is.EqualTo(401));
		Assert.That(response.NeedAuthorization, Is.True);
		Assert.That(handler.Sends, Is.EqualTo(1), "Ambiguous writes and protected 401 must not auto-retry.");
	}

	[TestCase(HttpStatusCode.BadRequest)]
	[TestCase(HttpStatusCode.InternalServerError)]
	[TestCase(HttpStatusCode.BadGateway)]
	public void Malformed_non_401_transport_error_throws_once_without_replay(HttpStatusCode status)
	{
		var handler = new ResponseHandler(status, "<html>malformed failure</html>");
		var service = new TestService(new ClientFactory(handler));
		Assert.ThrowsAsync<Newtonsoft.Json.JsonReaderException>(async () => { await service.Call(); });
		Assert.That(handler.Sends, Is.EqualTo(1), "Conservative malformed errors must not replay the operation.");
	}

	private sealed class ResponseHandler(HttpStatusCode status, string body) : HttpMessageHandler
	{
		public int Sends { get; private set; }
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Sends++;
			return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body), RequestMessage = request });
		}
	}

	private sealed class ClientFactory(ResponseHandler handler) : EasySDK.Mobile.RestClient.IHttpClientFactory
	{
		public HttpClient CreateHttpClient() => new(handler, disposeHandler: false);
	}

	private sealed class Tokens : ITokenProvider
	{
		public Task<bool> InvalidateToken() => throw new InvalidOperationException("No automatic token invalidation.");
		public Task<string?> TryGetAuthTokenAsync() => Task.FromResult<string?>("synthetic");
	}

	private sealed class TestService(ClientFactory factory)
		: HttpServiceBase(factory, NullLogger.Instance, new Tokens(), new Uri("https://example.invalid/"), autoReauthorize: false)
	{
		public Task<IResponse<string>> Call() => GetJsonAsync<string>("test");
	}
}
