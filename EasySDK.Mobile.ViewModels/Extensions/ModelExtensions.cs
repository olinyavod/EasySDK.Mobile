#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Converters;
using Newtonsoft.Json;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class ModelExtensions
{
	#region Constants

	public const double Kb = 1024;
	public const double Mb = Kb * Kb;
	public const double Gb = Kb * Mb;

	#endregion

	#region Public methods

	public static async Task<IResponse<TNewResult>> Convert<TResult, TNewResult>(this Task<IResponse<TResult>> responseTask, Func<TResult, TNewResult> convert)
	{
		var response = await responseTask;

		return response.Convert(convert);
	}

	public static async Task<IResponseList<TNewResult>> ConvertToList<TResult, TNewResult>(this Task<IResponse<TResult>> responseTask, Func<TResult, IEnumerable<TNewResult>> convert)
	{
		var response = await responseTask;

		return response.ConvertToList(convert);
	}

	public static string FileSizeToString(this long fileSize)
	{
		return fileSize switch
		{
			{} d when d > Gb => string.Format(Properties.Resources.GBFormat, fileSize/ Gb),
			{} d when  d> Mb => string.Format(Properties.Resources.MBormat, fileSize/ Mb),
			{} d when  d> Kb => string.Format(Properties.Resources.KBFormat, fileSize/ Kb),

			_ => string.Format(Properties.Resources.BFormat, fileSize)
		};
	}

	public static string ToUrlArgs(this string? args) => Uri.EscapeDataString(args ?? string.Empty);
	
	public static Dictionary<string, string> GetPropertiesMap(this Type? type)
	{
		var map = new Dictionary<string, string>(new IgnoreCaseComparer());
		if (type is null)
			return map;

		var properties = type.GetProperties();

		foreach (var property in properties)
		{
			foreach (var attribute in property.GetCustomAttributes(true))
			{
				switch (attribute)
				{
					case JsonPropertyAttribute {PropertyName: { } jp}:
						map[jp] = property.Name;
						break;

					case DataMemberAttribute {IsNameSetExplicitly:true, Name: { } dmn}:
						map[dmn] = property.Name;
						break;

					default:
						map[property.Name] = property.Name;
						break;
				}
			}
		}

		return map;
	}

	public static string? ToFirstUpper(this string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
			return text;

		var first = char.ToUpper(text[0]);

		if (text.Length < 2)
			return first.ToString();

		return first + text.Substring(1);
	}

	private static readonly Regex _clearPhoneRegex = new
	(
		@"[^\d\+]",
		RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled
	);

	public static string? ClearPhone(this string? phone)
	{
		if(string.IsNullOrWhiteSpace(phone))
			return phone;

		try
		{
			return _clearPhoneRegex.Replace(phone, "");
		}
		catch(Exception ex)
		{
			Debug.WriteLine("Clear phone error");
			return phone;
		}
	}

	#endregion
}