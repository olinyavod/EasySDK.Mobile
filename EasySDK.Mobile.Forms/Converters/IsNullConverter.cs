using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Converters;

public class IsNullConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value switch
		{
			null => true,
			string s => string.IsNullOrWhiteSpace(s),

			_ => false,
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}