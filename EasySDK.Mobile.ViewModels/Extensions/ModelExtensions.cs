using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class ModelExtensions
{
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

	#endregion

	public const double Kb = 1024;
	public const double Mb = Kb * Kb;
	public const double Gb = Kb * Mb;

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
}