using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Converters;

public class EmptyStringConverter : IValueConverter
{
	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value?.ToString() is { } str
		       && !string.IsNullOrWhiteSpace(str)
			? value
			: parameter;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	#endregion
}