using EasySDK.Mobile.Forms.Behaviors;
using EasySDK.Mobile.Forms.Effects;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Controls;

public class DataDatePicker : DatePicker, IDataField
{
	#region Private fields

	private readonly DataFieldBehavior _dataFieldBehavior;

	#endregion

	#region Properties

	#region DependencyProperty FieldName

	public static readonly BindableProperty FieldNameProperty =
		BindableProperty.Create(nameof(FieldName), typeof(string), typeof(DataDatePicker), default(string));

	public string FieldName
	{
		get => (string) GetValue(FieldNameProperty);
		set => SetValue(FieldNameProperty, value);
	}

	#endregion // DependencyProperty FieldName

	#endregion

	#region ctor

	public DataDatePicker()
	{
		Effects.Add(new DataErrorEffect());
		Effects.Add(new EntryIconEffect());

		_dataFieldBehavior = new DataFieldBehavior(this);
	}

	#endregion
}