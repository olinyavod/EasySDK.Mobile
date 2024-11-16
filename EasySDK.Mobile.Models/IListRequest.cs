namespace EasySDK.Mobile.Models
{
	public interface IListRequest
	{
		class EmptyRequest : IListRequest
		{
			public int Offset { get; set; }
			public int Count { get; set; }
			public string? Search { get; set; }
		}

		public static IListRequest Empty { get; } = new EmptyRequest();

		#region Properties

		int Offset { get; set; }

		int Count { get; set; }

		string Search { get; set; }

		#endregion
	}
}