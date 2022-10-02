using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.Tests.Services;

public class TestTokenProvider : ITokenProvider
{
	public bool InvalidateToken()
	{
		
	}

	public Task<string?> TryGetAuthTokenAsync()
	{
		throw new NotImplementedException();
	}
}