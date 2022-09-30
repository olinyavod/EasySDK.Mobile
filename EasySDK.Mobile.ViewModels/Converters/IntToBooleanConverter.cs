using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters
{
	public class IntToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!int.TryParse(parameter?.ToString(), out var p))
				p = 0;

			return value switch
			{
				int i => i > p,
				uint i => i > p,
				long l => l > p,
				ulong l => l > (ulong) p,
				byte b => b > p,
				sbyte b => b > p,

				_ => false

			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
