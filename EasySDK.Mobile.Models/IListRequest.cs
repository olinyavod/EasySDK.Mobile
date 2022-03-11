namespace EasySDK.Mobile.Models
{
	public interface IListRequest
	{
		#region Properties

		int Offset { get; set; }

		int Count { get; set; }

		string Search { get; set; }

		#endregion
	}
}