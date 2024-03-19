using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Tests.Services;

public class TestTokenProvider : ITokenProvider
{
	public Task<bool> InvalidateToken()
	{
		return Task.FromResult(false);
	}

	public Task<string?> TryGetAuthTokenAsync()
	{
		throw new NotImplementedException();
	}
}