using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace EasySDK.Mobile.ViewModels.Converters;

public class StringToGeometryConverter : IValueConverter
{
	private static readonly PathGeometryConverter GeometryConverter = new();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return GeometryConverter.ConvertFromInvariantString(value?.ToString());
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}