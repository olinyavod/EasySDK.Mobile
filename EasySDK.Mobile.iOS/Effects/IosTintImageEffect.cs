using System.ComponentModel;
using EasySDK.Mobile.Forms.Effects;
using EasySDK.Mobile.iOS.Effects;
using Xamarin.Forms;
using Foundation;

[assembly: ExportEffect(typeof(IosTintImageEffect), nameof(TintImageEffect))]
namespace EasySDK.Mobile.iOS.Effects
{
    using System;
    using System.Linq;
    using UIKit;
    using Xamarin.Forms.Platform.iOS;

    [Preserve(AllMembers = true)]
    public class IosTintImageEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateColors();
        }

        private void UpdateColors()
        {
	        try
	        {
                var effect = (TintImageEffect)Element.Effects.FirstOrDefault(e => e is TintImageEffect);

		        if (effect == null || Control is not UIImageView image)
			        return;

		        image.Image = image?.Image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
		        image.TintColor = TintImageEffect.GetTintColor(Element).ToUIColor();
	        }
	        catch (Exception ex)
	        {
		        System.Diagnostics.Debug.WriteLine(
			        $"An error occurred when setting the {typeof(TintImageEffect)} effect: {ex.Message}\n{ex.StackTrace}");
	        }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            if(args.PropertyName == TintImageEffect.TintColorProperty.PropertyName)
                UpdateColors();
            else if(args.PropertyName == Image.SourceProperty.PropertyName)
                UpdateColors();

	        base.OnElementPropertyChanged(args);
        }

        protected override void OnDetached()
        {
        }
    }
}