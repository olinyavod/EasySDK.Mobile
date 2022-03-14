using System;
using System.Collections.Generic;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.FakeProviders.Models;

class FakeResponse : IResponse
{
	#region Properties

	public bool HasError => ErrorCode > 0;

	public int ErrorCode { get; set; }

	public string ErrorMessage { get; set; }

	public Dictionary<string, IEnumerable<string>> ErrorMessages { get; } = new();

	public bool NeedAuthorization => ErrorCode == ResponseErrorCodes.Unauthorized;

	#endregion

	public static IResponse<TResult> FromResult<TResult>(TResult result) => new FakeResponse<TResult>(result);

	public static IResponse<TResult> FromErrorCode<TResult>(int errorCode) => new FakeResponse<TResult>
	{
		ErrorCode = errorCode
	};
}

class FakeResponse<TResult> : FakeResponse, IResponse<TResult>
{
	#region Properties

	public TResult Result { get; }

	#endregion

	#region ctor

	public FakeResponse()
	{
		
	}

	public FakeResponse(TResult result)
	{
		Result = result;
	}

	public IResponse<TNewResult> Convert<TNewResult>(Func<TResult, TNewResult> convert) => new FakeResponse<TNewResult>(convert(Result));

	#endregion
}