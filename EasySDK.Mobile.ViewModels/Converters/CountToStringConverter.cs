using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters;

public class CountToStringConverter : IValueConverter
{
	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var max = parameter switch
		{
			int m => m,
			string m when int.TryParse(m, out var i) => i,

			_ => 99
		};

		var v = value switch
		{
			int i => i,
			string s when int.TryParse(s, out var i) => i,

			_ => 0
		};

		return v <= max ? v.ToString() : $"{max}+";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	#endregion
}