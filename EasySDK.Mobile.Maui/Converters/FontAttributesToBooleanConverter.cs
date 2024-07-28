using System.Globalization;

namespace EasySDK.Mobile.Maui.Converters;

public class FontAttributesToBooleanConverter : IValueConverter, IMarkupExtension
{
	public FontAttributes FontAttribute { get; set; }

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if(value is FontAttributes fontAttributes)
			return fontAttributes.HasFlag(FontAttribute);

		return false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ProvideValue(IServiceProvider serviceProvider)
	{
		return this;
	}
}