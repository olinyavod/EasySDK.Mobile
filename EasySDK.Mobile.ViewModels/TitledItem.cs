namespace EasySDK.Mobile.ViewModels;

public record TitledItem<TItem>(string Title, TItem Value)
{
	#region Properties

	public string Title { get; } = Title;

	public TItem Value { get; } = Value;

	#endregion

	#region Public methods

	public override string ToString()
	{
		return Title;
	}

	#endregion
}