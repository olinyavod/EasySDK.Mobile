using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters;

public class BytesToFileSizeConverter : IValueConverter
{
	#region Private fields

	private const double Kb = 1024;
	private const double Mb = Kb * Kb;
	private const double Gb = Kb * Mb;

	#endregion

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

	private string BytesToString(long value)
	{
		return value switch
		{
			{} d when d > Gb => string.Format(Properties.Resources.GBFormat, value/Gb),
			{} d when  d> Mb => string.Format(Properties.Resources.MBormat, value/Mb),
			{} d when  d> Kb => string.Format(Properties.Resources.KBFormat, value/Kb),

			_ => string.Format(Properties.Resources.BFormat, value)
		};
	}

	#endregion
}