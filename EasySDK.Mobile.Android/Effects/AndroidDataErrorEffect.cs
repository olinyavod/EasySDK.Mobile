using System.ComponentModel;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using EasySDK.Mobile.Android.Effects;
using EasySDK.Mobile.ViewModels.Effects;
using Google.Android.Material.TextField;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(AndroidDataErrorEffect), nameof(DataErrorEffect))] 

namespace EasySDK.Mobile.Android.Effects;

public class AndroidDataErrorEffect : PlatformEffect
{
	#region Protected methods

	protected override void OnAttached()
	{
		UpdateError();
	}

	protected override void OnDetached()
	{
		SetError(string.Empty);
	}

	protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(e);

		if (e.PropertyName == DataErrorEffect.ErrorProperty.PropertyName)
			UpdateError();
	}

	#endregion

	#region Private methods

	private void UpdateError()
	{
		var message = DataErrorEffect.GetError(Element);

		SetError(message);
	}

	private void SetError(string text)
	{
		switch (Control)
		{
			case TextInputLayout inputLayout:
				inputLayout.Error = text;
				inputLayout.ErrorEnabled = !string.IsNullOrWhiteSpace(text);
				break;

			case EditText editText:
				editText.Error = text;
				break;

		}
	}

	#endregion
}