using EasySDK.Mobile.Android.Effects;
using EasySDK.Mobile.Forms.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidShadowEffect), nameof(ShadowEffect))]

namespace EasySDK.Mobile.Android.Effects;

public class AndroidShadowEffect : PlatformEffect
{
	protected override void OnAttached()
	{
		if (ShadowEffect.GetElevation(Element) is { } elevation)
			Control.SetElevation(elevation * Control.Resources.DisplayMetrics.Density);
	}

	protected override void OnDetached()
	{
		
	}
}