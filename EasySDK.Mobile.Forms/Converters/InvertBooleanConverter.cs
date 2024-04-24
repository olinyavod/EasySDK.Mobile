using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Converters;

public class InvertBooleanConverter : IValueConverter
{
	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return InvertValue(value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return InvertValue(value);
	}

	#endregion

	#region Private methods

	private bool InvertValue(object value) => value switch
	{
		bool b => !b,
		_ => false
	};

	#endregion
}