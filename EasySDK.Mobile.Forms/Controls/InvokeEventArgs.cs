using System;

namespace EasySDK.Mobile.Forms.Controls;

public class InvokeEventArgs : EventArgs
{
	#region Properties

	public string Data { get; }

	#endregion

	#region ctor

	public InvokeEventArgs(string data)
	{
		Data = data;
	}

	#endregion
}