using System;
using System.Collections.Generic;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.RestClient.Cpnverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.RestClient.Models;

public class HttpResponse : IResponse
{
	#region Properties

	public bool HasError => ErrorCode != 0;

	[JsonProperty("errorCode")]
	public int ErrorCode { get; set; }

	[JsonProperty("errorMessage")]
	public string ErrorMessage { get; set; }

	[JsonProperty("errorMessages")]
	[JsonConverter(typeof(ErrorsJsonConverter))]
	public Dictionary<string, IEnumerable<string>> ErrorMessages { get; set; }

	public bool NeedAuthorization => ErrorCode == ResponseErrorCodes.Unauthorized;

	[JsonProperty("_meta")]
	public JToken Metadata { get; set; }

	#endregion

	#region ctor

	public HttpResponse()
	{
		
	}

	public HttpResponse(IResponse response)
	{
		ErrorCode = response.ErrorCode;
		ErrorMessages = response.ErrorMessages;
		ErrorMessage = response.ErrorMessage;
	}

	#endregion

	#region Public methods

	public static IResponse<TResult> Success<TResult>(TResult result) => new HttpResponse<TResult>(result);

	public static IResponse<TResult> Fail<TResult>
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

public class HttpResponse<TResult> : HttpResponse, IResponse<TResult>
{
	#region Properties

	[JsonProperty("result")]
	public TResult Result { get; set; }

	#endregion

	#region ctor

	public HttpResponse()
	{
	}

	public HttpResponse(IResponse response)
		: base(response)
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
		ErrorMessages = response.ErrorMessages;
	}

	#endregion

	#region Public methods

	public IResponse<TNewResult> Convert<TNewResult>(Func<TResult, TNewResult> convert) => HasError
		? new HttpResponse<TNewResult>(this)
		: Success(convert(Result));

	public IResponseList<TNewResult> ConvertToList<TNewResult>(Func<TResult, IEnumerable<TNewResult>> convert)
	{
		return HasError
			? new HttpResponseList<TNewResult>(this)
			: new HttpResponseList<TNewResult>(convert(Result));
	}

	#endregion
}

public class HttpResponseList<TResult> : HttpResponse<IEnumerable<TResult>>, IResponseList<TResult>
{
	#region Properties
	
	[JsonIgnore]
	public int TotalCount { get; set; }

	#endregion

	#region ctor

	public HttpResponseList()
	{
		
	}

	public HttpResponseList(IResponse response)
		: base(response)
	{

	}

	public HttpResponseList(IEnumerable<TResult> result)
	{
		Result = result;
	}

	#endregion
}