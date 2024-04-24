using System;
using System.Globalization;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Converters;

public class BytesToFileSizeConverter : IValueConverter
{
	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value switch
		{
			long d => BytesToString(d),
			int d => BytesToString(d),

			_ => value
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	#endregion
	
	#region Private methods

	private string BytesToString(long value) => value.FileSizeToString();

	#endregion
}