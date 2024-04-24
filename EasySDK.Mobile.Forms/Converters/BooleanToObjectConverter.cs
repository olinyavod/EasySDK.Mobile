using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters;

public class BooleanToObjectConverter : IValueConverter
{
	#region Properties

	public object TrueValue { get; set; }

	public object FalseValue { get; set; }

	public object NullValue { get; set; }

	#endregion

	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
	{
		true => TrueValue,
		false => FalseValue,
		null => NullValue,

		_ => value
	};

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	#endregion
}