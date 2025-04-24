using System.Text.RegularExpressions;

namespace EasySDK.Mobile.Maui.Extensions;

public static partial class HtmlEntityProcessor
{
	[GeneratedRegex(@"&(?:#(\d+)|#x([\da-fA-F]+)|([a-zA-Z][\w\d]*));", RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.CultureInvariant)]
	private static partial Regex HtmlEntityRegex();

	public static string DecodeHtmlEntities(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return input;
        
		var regex = HtmlEntityRegex();

		return regex.Replace(input, match =>
		{
			var numericValue = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : (
				match.Groups[2].Success ? Convert.ToInt32(match.Groups[2].Value, 16) : -1);
            
			if (numericValue != -1)
				return char.ConvertFromUtf32(numericValue);

			return match.Groups[3].Value switch
			{
				"nbsp" => Environment.NewLine,
				"lt" => "<",
				"gt" => ">",
				"amp" => "&",
				"quot" => "\"",
				_ => match.Value
			};
		});
	}
}