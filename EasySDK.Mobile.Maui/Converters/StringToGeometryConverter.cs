using System.Globalization;
using Microsoft.Maui.Controls.Shapes;

namespace EasySDK.Mobile.Maui.Converters;

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