using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using EasySDK.Mobile.Forms.Controls;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Behaviors;

abstract class DataFieldBehaviorBase
{
	#region Private fields

	private readonly Element _element;
	private INotifyDataErrorInfo _dataErrorInfo;

	#endregion

	#region ctor

	protected DataFieldBehaviorBase(Element element)
	{
		_element = element;
		element.BindingContextChanged += ElementOnBindingContextChanged;
	}

	#endregion

	#region Protected methods

	protected abstract void ErrorOnChanged(Element element, IEnumerable errors);

	#endregion

	#region Private methods

	private void ElementOnBindingContextChanged(object sender, EventArgs e)
	{
		if (_dataErrorInfo != null)
			_dataErrorInfo.ErrorsChanged -= DataErrorInfoOnErrorsChanged;

		var element = (Element) sender;

		if (element.BindingContext is INotifyDataErrorInfo dataErrorInfo)
		{
			_dataErrorInfo = dataErrorInfo;
			_dataErrorInfo.ErrorsChanged += DataErrorInfoOnErrorsChanged;
		}
	}

	private void DataErrorInfoOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
	{
		if (_element is not IDataField dataField
		    || dataField.FieldName != e.PropertyName)
			return;

		var dataInfo = (INotifyDataErrorInfo) sender;
		var errors = dataInfo.GetErrors(e.PropertyName) ?? Enumerable.Empty<object>();

		ErrorOnChanged(_element, errors);
	}

	#endregion
}