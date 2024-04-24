using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Converters;

public class IntScaleToDoubleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is int i && parameter is double d)
			return i * d;

		return 0.0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}