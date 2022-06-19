using System;
using System.ComponentModel;
using Android.Graphics;
using Android.Widget;
using EasySDK.Mobile.Android.Effects;
using EasySDK.Mobile.ViewModels.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidTintImageEffect), nameof(TintImageEffect))]  
namespace EasySDK.Mobile.Android.Effects;

public class AndroidTintImageEffect : PlatformEffect  
{
	#region Private fields

	private PorterDuffColorFilter _colorFilter;

	#endregion

	#region Protected methods

	protected override void OnAttached()  
	{  
		UpdateColor();
	}

	protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
	{
		base.OnElementPropertyChanged(args);

		if (args.PropertyName == TintImageEffect.TintColorProperty.PropertyName)
			UpdateColor();
	}

	protected override void OnDetached()
	{
		_colorFilter?.Dispose();
	}

	#endregion

	#region Private methods

	private void UpdateColor()
	{
		try  
		{
			if (Control is not ImageView image)  
				return;

			_colorFilter?.Dispose();
			var color = TintImageEffect.GetTintColor(Element);
			_colorFilter = new PorterDuffColorFilter(color.ToAndroid(), PorterDuff.Mode.SrcIn);  
			image.SetColorFilter(_colorFilter);  
		}  
		catch (Exception ex)  
		{  
                  
		}  
	}

	#endregion
}