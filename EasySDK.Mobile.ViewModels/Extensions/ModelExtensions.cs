using System;
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

	#endregion
}