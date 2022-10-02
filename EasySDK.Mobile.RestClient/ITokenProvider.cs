#nullable enable

using System.Threading.Tasks;

namespace EasySDK.Mobile.RestClient;

public interface ITokenProvider
{
	bool InvalidateToken();

	Task<string?> TryGetAuthTokenAsync();
}