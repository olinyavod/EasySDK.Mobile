using System.Collections;
using System.Linq;
using EasySDK.Mobile.ViewModels.Behaviors;
using EasySDK.Mobile.ViewModels.Effects;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public class ErrorLabel : Label, IDataField
{
	#region Nested classes

	class ErrorLabelDataFieldBehavior : DataFieldBehaviorBase
	{
		#region ctpr

		public ErrorLabelDataFieldBehavior(Element element)
			: base(element)
		{
		}

		#endregion

		#region Protected methods

		protected override void ErrorOnChanged(Element element, IEnumerable errors)
		{
			var label = (Label) element;

			label.Text = errors.OfType<object>()
				.Select(i => i.ToString())
				.FirstOrDefault() ?? string.Empty;
		}

		#endregion
	}

	#endregion

	#region Private fields

	private ErrorLabelDataFieldBehavior _dataFieldBehavior;

	#endregion

	#region Properties

	public static readonly BindableProperty FieldNameProperty = BindableProperty.Create(nameof(FieldName), typeof(string), typeof(ErrorLabel), default(string));

	public string FieldName
	{
		get => (string) GetValue(FieldNameProperty);
		set => SetValue(FieldNameProperty, value);
	}

	#endregion

	#region ctor

	public ErrorLabel()
	{
		Effects.Add(new EntryIconEffect());

		_dataFieldBehavior = new ErrorLabelDataFieldBehavior(this);
	}

	#endregion
}
