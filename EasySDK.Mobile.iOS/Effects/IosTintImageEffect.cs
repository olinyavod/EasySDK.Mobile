using EasySDK.Mobile.iOS.Effects;
using Xamarin.Forms;
using EasySDK.Mobile.ViewModels.Effects;

[assembly: ExportEffect(typeof(IosTintImageEffect), nameof(TintImageEffect))]

namespace EasySDK.Mobile.iOS.Effects
{
    using System;
    using System.Linq;
    using UIKit;
    using ViewModels.Effects;
    using Xamarin.Forms.Platform.iOS;

    public class IosTintImageEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (TintImageEffect)Element.Effects.FirstOrDefault(e => e is TintImageEffect);

                if (effect == null || !(Control is UIImageView image))
                    return;

                image.Image = image.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                image.TintColor = TintImageEffect.GetTintColor(Element).ToUIColor();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"An error occurred when setting the {typeof(TintImageEffect)} effect: {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected override void OnDetached()
        {
        }
    }
}