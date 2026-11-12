using System.Net;
using EasySDK.Mobile.RestClient;
using EasySDK.Mobile.RestClient.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasySDK.Mobile.ViewModels.Tests.Services;

public class HttpServiceBaseErrorResponseTests
{
	[TestCase(@"{""message"":""Primary"",""errorMessage"":""Secondary"",""description"":""Description""}", "Primary")]
	[TestCase(@"{""message"":""Primary""}", "Primary")]
	[TestCase(@"{""errorMessage"":""Secondary"",""description"":""Description""}", "Secondary")]
	[TestCase(@"{""message"":null,""errorMessage"":""Secondary"",""description"":""Description""}", "Secondary")]
	[TestCase(@"{""description"":""Description""}", "Description")]
	[TestCase(@"{""message"":null,""description"":""Description""}", "Description")]
	[TestCase(@"{""errorMessage"":null,""description"":""Description""}", "Description")]
	[TestCase(@"{""message"":null,""errorMessage"":null,""description"":""Description""}", "Description")]
	public void CreateErrorResponse_selects_first_string_message(string content, string expectedMessage)
	{
		var result = CreateErrorResponse(content);

		AssertResponse(result, expectedMessage, HttpStatusCode.BadRequest);
	}

	[TestCase(@"{""message"":{},""errorMessage"":""Secondary""}", "Secondary")]
	[TestCase(@"{""message"":[],""errorMessage"":""Secondary""}", "Secondary")]
	[TestCase(@"{""message"":42,""errorMessage"":""Secondary""}", "Secondary")]
	[TestCase(@"{""message"":true,""errorMessage"":""Secondary""}", "Secondary")]
	[TestCase(@"{""message"":{},""errorMessage"":[],""description"":""Description""}", "Description")]
	[TestCase(@"{""message"":42,""errorMessage"":false,""description"":""Description""}", "Description")]
	[TestCase(@"{""message"":{},""errorMessage"":"""",""description"":""Description""}", "")]
	public void CreateErrorResponse_skips_non_string_messages(string content, string expectedMessage)
	{
		var result = CreateErrorResponse(content);

		AssertResponse(result, expectedMessage, HttpStatusCode.BadRequest);
	}

	[TestCase("{}")]
	[TestCase(@"{""message"":null}")]
	[TestCase(@"{""errorMessage"":null}")]
	[TestCase(@"{""description"":null}")]
	[TestCase(@"{""message"":null,""errorMessage"":null}")]
	[TestCase(@"{""message"":null,""description"":null}")]
	[TestCase(@"{""errorMessage"":null,""description"":null}")]
	[TestCase(@"{""message"":null,""errorMessage"":null,""description"":null}")]
	[TestCase("{\r\n  \"unrelated\": \"Unchanged formatting\"\r\n}\r\n")]
	[TestCase(@"{""message"":{},""errorMessage"":[],""description"":42}")]
	[TestCase(@"{""message"":true,""errorMessage"":false,""description"":null}")]
	[TestCase(@"{""Message"":""Different case""}")]
	[TestCase(@"{""nested"":{""message"":""Not a root field""}}")]
	[TestCase(@"[{""message"":""Not a root field""}]")]
	[TestCase(@"""Root string""")]
	[TestCase("42")]
	[TestCase("true")]
	[TestCase("null")]
	public void CreateErrorResponse_preserves_raw_content_when_no_message_is_a_string(string content)
	{
		var result = CreateErrorResponse(content);

		AssertResponse(result, content, HttpStatusCode.BadRequest);
	}

	[TestCase(@"{""message"":"""",""errorMessage"":""Secondary"",""description"":""Description""}", "")]
	[TestCase(@"{""message"":"" \t "",""errorMessage"":""Secondary"",""description"":""Description""}", " \t ")]
	[TestCase(@"{""message"":null,""errorMessage"":"""",""description"":""Description""}", "")]
	[TestCase(@"{""message"":null,""errorMessage"":"" \t "",""description"":""Description""}", " \t ")]
	[TestCase(@"{""message"":null,""errorMessage"":null,""description"":""""}", "")]
	[TestCase(@"{""message"":null,""errorMessage"":null,""description"":"" \t ""}", " \t ")]
	public void CreateErrorResponse_preserves_selected_empty_or_whitespace_message(string content, string expectedMessage)
	{
		var result = CreateErrorResponse(content);

		AssertResponse(result, expectedMessage, HttpStatusCode.BadRequest);
	}

	[TestCase(@"{""message"":""\u041e\u0448\u0438\u0431\u043a\u0430""}")]
	[TestCase(@"{""errorMessage"":""\u041e\u0448\u0438\u0431\u043a\u0430""}")]
	[TestCase(@"{""description"":""\u041e\u0448\u0438\u0431\u043a\u0430""}")]
	public void CreateErrorResponse_decodes_json_unicode_escapes(string content)
	{
		var result = CreateErrorResponse(content);

		AssertResponse(result, "Ошибка", HttpStatusCode.BadRequest);
	}

	[TestCase(HttpStatusCode.BadRequest)]
	[TestCase(HttpStatusCode.Unauthorized)]
	[TestCase(HttpStatusCode.NotFound)]
	[TestCase(HttpStatusCode.InternalServerError)]
	public void CreateErrorResponse_preserves_http_status_and_does_not_populate_other_response_fields(HttpStatusCode statusCode)
	{
		const string content = @"{""errorMessage"":""Failure"",""description"":""Not a separate DTO value"",""errorMessages"":{""field"":""Not mapped""},""code"":0,""errorCode"":200,""result"":""Not populated"",""_meta"":{""total"":10}}";

		var result = CreateErrorResponse(content, statusCode);

		AssertResponse(result, "Failure", statusCode);
	}

	private static HttpResponse<string> CreateErrorResponse(string content, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.invalid/error-response-test");
		using var response = new HttpResponseMessage(statusCode) { RequestMessage = request };
		var service = new TestHttpService();

		return service.ParseErrorResponse<HttpResponse<string>>(response, content);
	}

	private static void AssertResponse(HttpResponse<string> result, string expectedMessage, HttpStatusCode statusCode)
	{
		Assert.That(result.ErrorMessage, Is.EqualTo(expectedMessage));
		Assert.That(result.ErrorCode, Is.EqualTo((int) statusCode));
		Assert.That(result.HasError, Is.True);
		Assert.That(result.ErrorDescription, Is.Null);
		Assert.That(result.ErrorMessages, Is.Null);
		Assert.That(result.Metadata, Is.Null);
		Assert.That(result.Result, Is.Null);
	}

	private sealed class TestHttpService : HttpServiceBase
	{
		public TestHttpService()
			: base(new TestHttpClientFactory(), NullLogger.Instance, new TestTokenProvider(), new Uri("https://example.invalid/"))
		{
		}

		public TResponse ParseErrorResponse<TResponse>(HttpResponseMessage response, string content)
			where TResponse : HttpResponse, new() => CreateErrorResponse<TResponse>(response, content);
	}

	private sealed class TestHttpClientFactory : IHttpClientFactory
	{
		public HttpClient CreateHttpClient() => throw new InvalidOperationException("Error parsing must not create an HTTP client.");
	}

	private sealed class TestTokenProvider : ITokenProvider
	{
		public Task<bool> InvalidateToken() => throw new InvalidOperationException("Error parsing must not invalidate a token.");

		public Task<string?> TryGetAuthTokenAsync() => throw new InvalidOperationException("Error parsing must not request a token.");
	}
}
