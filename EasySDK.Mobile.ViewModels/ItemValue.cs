namespace EasySDK.Mobile.ViewModels;

public class ItemValue<TValue>(TValue value, string? title)
{
	#region Properties

	public TValue Value { get; } = value;

	public string? Title { get; } = title;

	#endregion

	public static implicit operator TValue?(ItemValue<TValue> item) => item.Value ?? default;

	#region Public methods

	public override string ToString() => Title ?? base.ToString()!;

	#endregion
}