using System;
using AndroidX.Activity.Result;
using Object = Java.Lang.Object;

namespace EasySDK.Mobile.Android;

public class ActivityResultHandler : Java.Lang.Object, IActivityResultCallback
{
	#region Private fields

	private readonly Action<object> _onResult;

	#endregion

	#region ctor

	public ActivityResultHandler()
	{

	}

	public ActivityResultHandler(Action<object> onResult)
		: this()
	{
		_onResult = onResult;
	}

	#endregion

	#region Public methods

	public void OnActivityResult(Object result)
	{
		_onResult?.Invoke(result);
	}

	#endregion
}