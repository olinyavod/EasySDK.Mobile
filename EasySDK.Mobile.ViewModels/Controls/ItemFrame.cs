using System;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public class ItemFrame : Frame
{
	#region Events

	public event EventHandler Click;

	#endregion

	#region Properties

	#region DependencyProperty Clickable

	public static readonly BindableProperty ClickableProperty = BindableProperty.Create(nameof(Clickable), typeof(bool), typeof(ItemFrame), true);

	public bool Clickable
	{
		get => (bool) GetValue(ClickableProperty);
		set => SetValue(ClickableProperty, value);
	}

	#endregion // DependencyProperty Clickable

	#endregion

	#region Public methods

	public void RaiseClickEvent()
	{
		Click?.Invoke(this, EventArgs.Empty);
	}

	#endregion
}