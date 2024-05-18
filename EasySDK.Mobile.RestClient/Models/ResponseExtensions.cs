using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.RestClient.Models;

public static class ResponseExtensions
{
	public static string? GetErrorMessage(this IResponse? response) =>
		response?.ErrorMessage ?? response?.ErrorDescription;
}