using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.RestClient.Models;

public class ListRequest : IListRequest
{
	#region Properties

	public int Offset { get; set; } = 0;

	public int Count { get; set; } = 100;

	public string Search { get; set; }

	#endregion
}