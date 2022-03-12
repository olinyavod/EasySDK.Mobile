using System.Collections.Generic;
using Android.Content;
using Android.Webkit;
using EasySDK.Mobile.Android.Renderers;
using EasySDK.Mobile.ViewModels.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;

[assembly:ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace EasySDK.Mobile.Android.Renderers;

public class HybridWebViewRenderer : WebViewRenderer, IHybridWebViewDelegate
{
    #region Private fields

    private const string JavascriptFunction = "function invokeExternalAction(data){jsBridge.invokeAction(data);}";

    #endregion

    #region ctor

    public HybridWebViewRenderer(Context context)
	    : base(context)
    {

    }

    #endregion

    #region Public methods

    public void LoadUrl(string url, IDictionary<string, string> headers)
    {
	    Control.LoadUrl(url, headers);
    }

    #endregion

    #region Protected methods

    protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
    {
        base.OnElementChanged(e);

        if (e.NewElement is HybridWebView element)
	        Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
    }
    
    protected override WebViewClient GetWebViewClient() => new JavascriptWebViewClient(this, $"javascript:{JavascriptFunction}");

    #endregion
}