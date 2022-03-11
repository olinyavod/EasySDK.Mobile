using System;
using System.Collections.Generic;
using EasySDK.Mobile.Models;
using Newtonsoft.Json;

namespace EasySDK.Mobile.RestClient.Models;

class HttpResponse : IResponse
{
	#region Properties

	public bool HasError => ErrorCode != 0;

	[JsonProperty("errorCode")]
	public int ErrorCode { get; set; }

	[JsonProperty("errorMessage")]
	public string ErrorMessage { get; set; }

	[JsonProperty("errorMessages")]
	public Dictionary<string, IEnumerable<string>> ErrorMessages { get; set; }

	public bool NeedAuthorization { get; set; }

	#endregion

	#region Public methods

	public static HttpResponse<TResult> Success<TResult>(TResult result) => new(result);

	public static HttpResponse<TResult> Fail<TResult>
	(
		TResult defaultResult,
		int errorCode,
		string errorMessage,
		Action<HttpResponse<TResult>> edit = null
	)
	{
		var result = new HttpResponse<TResult>(defaultResult)
		{
			ErrorCode = errorCode,
			ErrorMessage = errorMessage
		};

		edit?.Invoke(result);

		return result;
	}

	#endregion
}

class HttpResponse<TResult> : HttpResponse, IResponse<TResult>
{
	#region Properties

	[JsonProperty("result")]
	public TResult Result { get; set; }

	#endregion

	#region ctor

	public HttpResponse()
	{
	}

	public HttpResponse(TResult result)
	{
		Result = result;
	}

	public HttpResponse(HttpResponse response)
	{
		ErrorCode = response.ErrorCode;
		ErrorMessage = response.ErrorMessage;
		NeedAuthorization = response.NeedAuthorization;
	}

	#endregion

	#region Public methods

	public IResponse<TNewResult> Convert<TNewResult>(Func<TResult, TNewResult> convert) => HasError
		? new HttpResponse<TNewResult>(this)
		: HttpResponse.Success(convert(Result));

	#endregion
}