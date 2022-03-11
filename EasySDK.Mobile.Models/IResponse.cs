using System;
using System.Collections.Generic;

namespace EasySDK.Mobile.Models;

public interface IResponse
{
	#region Properties

	bool HasError { get; }

	int ErrorCode { get; }

	string ErrorMessage { get; }

	Dictionary<string, IEnumerable<string>> ErrorMessages { get; }

	bool NeedAuthorization { get; }

	#endregion
}

public interface IResponse<out TResult> : IResponse
{
	#region Properties

	TResult Result { get; }

	#endregion

	#region Methods

	IResponse<TNewResult> Convert<TNewResult>(Func<TResult, TNewResult> convert);

	#endregion
}
