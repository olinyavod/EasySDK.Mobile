using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.Forms.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CheckBoxView
{
	#region Properties

	#region DependencyProperty Text

	public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CheckBoxView), default(string));

	public string Text
	{
		get => (string) GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	#endregion // DependencyProperty Text

	#region DependencyProperty IsChecked

	public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBoxView), default(bool));

	public bool IsChecked
	{
		get => (bool) GetValue(IsCheckedProperty);
		set => SetValue(IsCheckedProperty, value);
	}

	#endregion // DependencyProperty IsChecked

	#endregion

	#region ctor

	public CheckBoxView()
	{
		InitializeComponent();
	}

	#endregion

	protected override void OnPropertyChanged(string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		switch (propertyName)
		{
			case nameof(IsChecked):
				PART_CheckBox.IsChecked = IsChecked;
				break;

			case nameof(Text):
				PART_Label.Text = Text;
				break;
		}
	}

	#region Private methods

	private void LabelOnTapped(object sender, EventArgs e)
	{
		PART_CheckBox.IsChecked = !PART_CheckBox.IsChecked;
	}

	private void CheckBoxOnCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		IsChecked = e.Value;
	}

	#endregion
}