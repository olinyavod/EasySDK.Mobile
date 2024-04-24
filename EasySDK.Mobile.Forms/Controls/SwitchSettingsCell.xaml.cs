using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.Forms.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SwitchSettingsCell
{
	#region Properties

	#region DependencyProperty Text

	public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SwitchSettingsCell), default(string));

	public string Text
	{
		get => (string) GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	#endregion // DependencyProperty Text

	#region DependencyProperty Description

	public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(SwitchSettingsCell), default(string));

	public string Description
	{
		get => (string) GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	#endregion // DependencyProperty Description

	#region DependencyProperty IsToggled

	public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchSettingsCell), default(bool), defaultBindingMode:BindingMode.TwoWay);

	public bool IsToggled
	{
		get => (bool) GetValue(IsToggledProperty);
		set => SetValue(IsToggledProperty, value);
	}

	#endregion // DependencyProperty IsToggled

	#endregion

	#region ctor

	public SwitchSettingsCell()
	{
		InitializeComponent();
	}

	#endregion

	#region Private methods

	private void SwitchSettingsCellOnClick(object sender, EventArgs e)
	{
		ValueSwitch.IsToggled = !ValueSwitch.IsToggled;
	}

	#endregion
}