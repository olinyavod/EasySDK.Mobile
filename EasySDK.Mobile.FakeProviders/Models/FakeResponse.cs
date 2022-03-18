using System;
using System.Collections.Generic;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.FakeProviders.Models;

public class FakeResponse : IResponse
{
	#region Properties

	public bool HasError => ErrorCode > 0;

	public int ErrorCode { get; set; }

	public string ErrorMessage { get; set; }

	public Dictionary<string, IEnumerable<string>> ErrorMessages { get; } = new();

	public bool NeedAuthorization => ErrorCode == ResponseErrorCodes.Unauthorized;

	#endregion

	#region ctor

	public FakeResponse()
	{
		
	}

	public FakeResponse(IResponse response)
	{
		ErrorCode = response.ErrorCode;
		ErrorMessage = response.ErrorMessage;
		ErrorMessages = response.ErrorMessages;
	}

	#endregion

	#region Public methods

	public static IResponse<TResult> FromResult<TResult>(TResult result) => new FakeResponse<TResult>(result);

	public static IResponse<TResult> FromErrorCode<TResult>(int errorCode) => new FakeResponse<TResult>
	{
		ErrorCode = errorCode
	};

	public static IResponseList<TModel> FromResultList<TModel>(IEnumerable<TModel> items, int total) => new FakeResponseList<TModel>(items)
	{
		TotalCount = total
	};

	#endregion
}

class FakeResponse<TResult> : FakeResponse, IResponse<TResult>
{
	#region Properties

	public TResult Result { get; }

	#endregion

	#region ctor

	public FakeResponse(IResponse response)
		: base(response)
	{

	}

	public FakeResponse()
	{
		
	}

	public FakeResponse(TResult result)
	{
		Result = result;
	}

	public IResponse<TNewResult> Convert<TNewResult>(Func<TResult, TNewResult> convert) => !HasError
		? new FakeResponse<TNewResult>(convert(Result))
		: new FakeResponse<TNewResult>(this);

	public IResponseList<TNewResult> ConvertToList<TNewResult>(Func<TResult, IEnumerable<TNewResult>> convert) => HasError
			? new FakeResponseList<TNewResult>(this)
			: new FakeResponseList<TNewResult>(convert(Result));
	
	#endregion
}

class FakeResponseList<TResult> : FakeResponse<IEnumerable<TResult>>, IResponseList<TResult>
{
	#region Properties

	public int TotalCount { get; set; }

	#endregion

	#region ctor

	public FakeResponseList()
	{
		
	}

	public FakeResponseList(IResponse response)
		: base(response)
	{

	}

	public FakeResponseList(IEnumerable<TResult> result)
		: base(result)
	{

	}

	#endregion
}