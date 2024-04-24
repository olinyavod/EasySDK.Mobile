using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Effects;

public class TintImageEffect : RoutingEffect
{
	#region Properties

	#region AttachedProperty TintColor

	public static readonly BindableProperty TintColorProperty =
		BindableProperty.CreateAttached("TintColor", typeof(Color), typeof(TintImageEffect), default(Color));

	public static void SetTintColor(BindableObject element, Color value)
	{
		element.SetValue(TintColorProperty, value);
	}

	public static Color GetTintColor(BindableObject element)
	{
		return (Color) element.GetValue(TintColorProperty);
	}

	#endregion //AttachedProperty TintColor

	#endregion

	#region ctor

	public TintImageEffect()
		: base($"{nameof(EasySDK)}.{nameof(Mobile)}.{nameof(TintImageEffect)}")
	{

	}

	#endregion
}