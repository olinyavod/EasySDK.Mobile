using System.Globalization;

namespace EasySDK.Mobile.Maui.Converters;

public class InvertBooleanConverter : IValueConverter, IMarkupExtension
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value switch
		{
			bool b => !b,
			_ => false
		};
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value switch
		{
			bool b => !b,
			_ => false
		};
	}

	public object ProvideValue(IServiceProvider serviceProvider) => this;
}