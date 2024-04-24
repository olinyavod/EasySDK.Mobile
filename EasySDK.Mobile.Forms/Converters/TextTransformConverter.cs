using System;
using System.Globalization;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Converters;

public class TextTransformConverter : IValueConverter
{
	#region Public methods

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value switch
		{
			string s => TransformString(s, parameter), 
			_ => value
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Private fields

	private static string TransformString(string value, object parameter)
	{
		if(!Enum.TryParse(parameter?.ToString() ?? string.Empty, out TextTransform transform))
			transform = TextTransform.Default;

		return transform switch
		{
			TextTransform.Lowercase => value.ToLower(),
			TextTransform.Uppercase => value.ToUpper(),

			_ => value
		};
	}

	#endregion
}