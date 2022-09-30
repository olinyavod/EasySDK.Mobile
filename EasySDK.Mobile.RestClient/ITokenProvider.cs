#nullable enable

using System.Threading.Tasks;

namespace EasySDK.Mobile.RestClient;

public interface ITokenProvider
{
	string? Token { get; set; }

	Task<string?> TruGetTokenAsync();
}