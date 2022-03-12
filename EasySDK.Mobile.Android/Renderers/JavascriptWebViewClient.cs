using Android.Webkit;
using Xamarin.Forms.Platform.Android;

namespace EasySDK.Mobile.Android.Renderers;

public class JavascriptWebViewClient : FormsWebViewClient
{
	#region Private fields

	private readonly string _javascript;

	#endregion

	#region ctor

	public JavascriptWebViewClient(HybridWebViewRenderer renderer, string javascript) 
		: base(renderer)
	{
		_javascript = javascript;
	}

	#endregion

	#region Public methods

	public override void OnPageFinished(WebView view, string url)
	{
		base.OnPageFinished(view, url);
		view.EvaluateJavascript(_javascript, null);
	}

	#endregion
}