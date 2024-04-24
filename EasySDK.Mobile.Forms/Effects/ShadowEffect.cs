using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Effects;

public class ShadowEffect : RoutingEffect
{
	#region Properties

	#region AttachedProperty Elevation

	public static readonly BindableProperty ElevationProperty = BindableProperty.CreateAttached(
		"Elevation", typeof(float?), typeof(ShadowEffect), default(float));

	public static void SetElevation(BindableObject element, float? value)
	{
		element.SetValue(ElevationProperty, value);
	}

	public static float GetElevation(BindableObject element)
	{
		return (float) element.GetValue(ElevationProperty);
	}

	#endregion //AttachedProperty Elevation

	#endregion

	#region ctor

	public ShadowEffect() 
		: base($"{nameof(EasySDK)}.{nameof(EasySDK.Mobile)}.{nameof(ShadowEffect)}")
	{
	}

	#endregion
}