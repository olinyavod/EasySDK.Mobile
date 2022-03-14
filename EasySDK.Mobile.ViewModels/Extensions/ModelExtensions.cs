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
}