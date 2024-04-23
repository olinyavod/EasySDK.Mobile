using System.Windows.Input;
using EasySDK.Mobile.ViewModels.Behaviors;
using EasySDK.Mobile.ViewModels.Effects;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public class DataEntry : Entry, IDataField
{
	#region Private fields

	private readonly DataFieldBehavior _dataFieldBehavior;
	private readonly ICommand _clearCommand;

	#endregion

	#region Properties

	#region DependencyProperty FieldName

	public static readonly BindableProperty FieldNameProperty =
		BindableProperty.Create(nameof(FieldName), typeof(string), typeof(DataEntry), default(string));

	public string FieldName
	{
		get => (string) GetValue(FieldNameProperty);
		set => SetValue(FieldNameProperty, value);
	}

	#endregion // DependencyProperty FieldName

	#region DependencyProperty AllowClear

	public static readonly BindableProperty AllowClearProperty = BindableProperty.Create(nameof(AllowClear), typeof(bool), typeof(DataEntry), true);

	public bool AllowClear
	{
		get => (bool) GetValue(AllowClearProperty);
		set => SetValue(AllowClearProperty, value);
	}

	#endregion // DependencyProperty AllowClear

	#endregion

	#region ctor

	public DataEntry()
	{
		Effects.Add(new EntryIconEffect());
		Effects.Add(new DataErrorEffect());

		_dataFieldBehavior = new DataFieldBehavior(this);

		_clearCommand = new Command(OnClear);

		UpdateAllowClear();
	}

	#endregion

	#region Protected methods

	protected override void OnPropertyChanged(string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		switch (propertyName)
		{
			case nameof(AllowClear):
				UpdateAllowClear();
				break;
		}
	}

	#endregion

	#region Private methods

	private void UpdateAllowClear()
	{
		if (AllowClear)
		{
			EntryIconEffect.SetEndIcon(this, "ic_cancel_white_24px");
			EntryIconEffect.SetEndIconCommand(this, _clearCommand);
		}
		else
		{
			EntryIconEffect.SetEndIcon(this, null);
			EntryIconEffect.SetEndIconCommand(this, null);
		}
	}

	private void OnClear()
	{
		Text = string.Empty;
	}

	#endregion
}