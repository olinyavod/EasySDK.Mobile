using System.Collections.Generic;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.RestClient.Extensions;

public static class HttpRequestExtensions
{
	#region Public methods

	public static string ToUrl(this IListRequest request, string requestUrl)
	{
		if (request == null)
			return requestUrl;

		IEnumerable<string> Query()
		{
			yield return $"offset={request.Offset}";
			yield return $"limit={request.Count}";

			if (!string.IsNullOrWhiteSpace(request.Search))
				yield return $"search={request.Search}";
		}

		return $"{requestUrl}?{string.Join("&", Query())}";
	}

	#endregion
}