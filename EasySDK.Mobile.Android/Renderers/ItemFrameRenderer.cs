using System.ComponentModel;
using Android.Content;
using EasySDK.Mobile.Android.Renderers;
using EasySDK.Mobile.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(ItemFrame), typeof(ItemFrameRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace EasySDK.Mobile.Android.Renderers;

public class ItemFrameRenderer : MaterialFrameRenderer, View.IOnClickListener
{
	public ItemFrameRenderer(Context context)
		: base(context)
	{
		CardElevation = 10f * context.Resources.DisplayMetrics.Density;
		SetOnClickListener(this);

	}

	protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
	{
		
		UpdateCllckable();
		base.OnElementChanged(e);
	}

	protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(sender, e);

		if(e.PropertyName == ItemFrame.ClickableProperty.PropertyName)
			UpdateCllckable();
	}

	private void UpdateCllckable()
	{
		if (Element is ItemFrame frame && frame.Clickable)
		{
			var attrs = new int[] {Resource.Attribute.selectableItemBackground};
			var typedArray = Control.Context.ObtainStyledAttributes(attrs);
			var backgroundResource = typedArray.GetDrawable(0);

			Control.Foreground = backgroundResource;
		}
		else
			Control.Foreground = null;
	}

	public void OnClick(View? v)
	{
		if(Element is ItemFrame frame)
			frame.RaiseClickEvent();
	}
}