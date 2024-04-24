using System;
using Android.Webkit;
using EasySDK.Mobile.Forms.Controls;
using Java.Interop;

namespace EasySDK.Mobile.Android.Renderers;

public class JSBridge : Java.Lang.Object
{
	#region Private fields

	private readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

	#endregion

	#region ctor

	public JSBridge(HybridWebViewRenderer hybridRenderer)
	{
		hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
	}

	#endregion

	#region Public methods

	[JavascriptInterface]
	[Export("invokeAction")]
	public void InvokeAction(string data)
	{
		if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out var hybridRenderer))
		{
			((HybridWebView)hybridRenderer.Element).InvokeAction(data);
		}
	}

	#endregion
}