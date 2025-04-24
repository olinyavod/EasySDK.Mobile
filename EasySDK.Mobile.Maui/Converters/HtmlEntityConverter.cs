using System.Globalization;
using EasySDK.Mobile.Maui.Extensions;

namespace EasySDK.Mobile.Maui.Converters;

public class HtmlEntityConverter : IValueConverter, IMarkupExtension
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is string strValue)
			return HtmlEntityProcessor.DecodeHtmlEntities(strValue);
		return value;
	}
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ProvideValue(IServiceProvider serviceProvider)
	{
		return this;
	}
}