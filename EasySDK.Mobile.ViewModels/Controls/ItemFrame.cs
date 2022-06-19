using System;
using System.Windows.Input;
using EasySDK.Mobile.ViewModels.Extensions;
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

	#region DependencyProperty ClickCommand

	public static readonly BindableProperty ClickCommandProperty =
		BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(ItemFrame), default(ICommand));

	public ICommand ClickCommand
	{
		get => (ICommand) GetValue(ClickCommandProperty);
		set => SetValue(ClickCommandProperty, value);
	}

	#endregion // DependencyProperty ClickCommand

	#region DependencyProperty ClickCommandParameter

	public static readonly BindableProperty ClickCommandParameterProperty =
		BindableProperty.Create(nameof(ClickCommandParameter), typeof(object), typeof(ItemFrame), default(object));

	public object ClickCommandParameter
	{
		get => (object) GetValue(ClickCommandParameterProperty);
		set => SetValue(ClickCommandParameterProperty, value);
	}

	#endregion // DependencyProperty ClickCommandParameter

	#endregion

	#region Public methods

	public void RaiseClickEvent()
	{
		Click?.Invoke(this, EventArgs.Empty);
		ClickCommand?.TryExecute(ClickCommandParameter);
	}

	#endregion
}