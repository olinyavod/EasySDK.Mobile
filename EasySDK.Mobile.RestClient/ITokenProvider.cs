#nullable enable

using System.Threading.Tasks;

namespace EasySDK.Mobile.RestClient;

public interface ITokenProvider
{
	Task<bool> InvalidateToken();

	Task<string?> TryGetAuthTokenAsync();
}