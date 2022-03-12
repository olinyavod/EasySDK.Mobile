using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Effects;

public class DataErrorEffect : RoutingEffect
{
	#region Properties

	#region AttachedProperty Error

	public static readonly BindableProperty ErrorProperty = BindableProperty.CreateAttached(
		"Error", typeof(string), typeof(DataErrorEffect), default(string));

	public static void SetError(BindableObject element, string value)
	{
		element.SetValue(ErrorProperty, value);
	}

	public static string GetError(BindableObject element)
	{
		return (string) element.GetValue(ErrorProperty);
	}

	#endregion //AttachedProperty Error

	#endregion

	#region ctor

	public DataErrorEffect()
		: base($"{nameof(EasySDK)}.{nameof(Mobile)}.{nameof(DataErrorEffect)}")
	{

	}

	#endregion
}