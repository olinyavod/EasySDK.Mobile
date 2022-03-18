using System;
using System.Windows.Input;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ButtonSettingsCell
{
	#region Properties

	#region DependencyProperty Text

	public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ButtonSettingsCell), default(string));

	public string Text
	{
		get => (string) GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	#endregion // DependencyProperty Text

	#region DependencyProperty Discription

	public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(ButtonSettingsCell), default(string));

	public string Description
	{
		get => (string) GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	#endregion // DependencyProperty Discription

	#region DependencyProperty Command

	public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ButtonSettingsCell), default(ICommand));

	public ICommand Command
	{
		get => (ICommand) GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	#endregion // DependencyProperty Command

	#region DependencyProperty CommandParameter

	public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ButtonSettingsCell), default(object));

	public object CommandParameter
	{
		get => (object) GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	#endregion // DependencyProperty CommandParameter

	#endregion

	#region ctor

	public ButtonSettingsCell()
	{
		InitializeComponent();
	}

	#endregion

	#region Private methods

	private void ButtonSettingsCellOnClick(object sender, EventArgs e)
	{
		Command?.TryExecute(CommandParameter);
	}

	#endregion
}