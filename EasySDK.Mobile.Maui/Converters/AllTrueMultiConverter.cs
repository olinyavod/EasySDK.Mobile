using System.Globalization;

namespace EasySDK.Mobile.Maui.Converters;

public class AllTrueMultiConverter : IMultiValueConverter, IMarkupExtension
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		return values?.All(i => i is true) is true;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ProvideValue(IServiceProvider serviceProvider) => this;
}