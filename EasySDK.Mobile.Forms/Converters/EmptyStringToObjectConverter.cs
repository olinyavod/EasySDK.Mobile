using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters;

public class EmptyStringToObjectConverter : IValueConverter
{
	#region Properties

	public object EmptyValue { get; set; }

	public object NotEmptyValue { get; set; }

	#endregion

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
	{
		string str => string.IsNullOrWhiteSpace(str) ? EmptyValue : NotEmptyValue,

		_ => EmptyValue
	};

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}