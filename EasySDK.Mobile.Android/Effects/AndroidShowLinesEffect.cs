using System.ComponentModel;
using System.Linq;
using Android.Views;
using Android.Widget;
using EasySDK.Mobile.Android.Effects;
using EasySDK.Mobile.ViewModels.Effects;
using Google.Android.Material.TextField;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidShowLinesEffect), nameof(ShowLinesEffect))]

namespace EasySDK.Mobile.Android.Effects;

public class AndroidShowLinesEffect : PlatformEffect
{
	#region Protected methods

	protected override void OnAttached()
	{
		UpdateVerticalOptions();
		UpdateMinLines();
		UpdateMaxLines();
	}

	protected override void OnDetached()
	{
	}

	protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(e);

		if (e.PropertyName == ShowLinesEffect.MinLinesProperty.PropertyName)
			UpdateMinLines();
		else if (e.PropertyName == ShowLinesEffect.MaxLinesProperty.PropertyName)
			UpdateMaxLines();
		else if (e.PropertyName == ShowLinesEffect.FillVerticalProperty.PropertyName)
			UpdateVerticalOptions();
	}

	#endregion

	#region Private methods

	private void UpdateVerticalOptions()
	{
		if (!ShowLinesEffect.GetFillVertical(Element))
			return;
		
		switch (Control)
		{
			case TextInputLayout layout:
			{
				if (layout.EditText.LayoutParameters is FrameLayout.LayoutParams p)
				{
					layout.EditText.LayoutParameters = new FrameLayout.LayoutParams(p)
					{
						Width = ViewGroup.LayoutParams.MatchParent,
						Height = ViewGroup.LayoutParams.MatchParent,
						Gravity = GravityFlags.Fill
					};

				}
				break;
			}
				
		}
	}

	private void UpdateMaxLines()
	{
		if (ShowLinesEffect.GetMaxLines(Element) is not { } value)
			return;

		switch (Control)
		{
			case EditText edit:
				edit.SetMaxLines(value);
				break;

			case TextInputLayout layout:
				layout.EditText.SetMaxLines(value);
				break;
		}
	}

	private void UpdateMinLines()
	{
		if (ShowLinesEffect.GetMinLines(Element) is not { } value)
			return;

		switch (Control)
		{
			case EditText edit:
				edit.SetMinLines(value);
				break;

			case TextInputLayout layout:
				layout.EditText.SetMinLines(value);
				break;
		}
	}

	#endregion
}