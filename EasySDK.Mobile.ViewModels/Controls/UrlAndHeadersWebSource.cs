using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public class UrlAndHeadersWebSource : WebViewSource
{
	#region Properties

	#region DependencyProperty Url

	public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(Uri), typeof(UrlAndHeadersWebSource), default(Uri));

	public Uri Url
	{
		get => (Uri) GetValue(UrlProperty);
		set => SetValue(UrlProperty, value);
	}

	#endregion // DependencyProperty Url

	#region DependencyProperty Headers

	public static readonly BindableProperty HeadersProperty = BindableProperty.Create(nameof(Headers), typeof(Dictionary<string, string>), typeof(UrlAndHeadersWebSource), new Dictionary<string, string>());

	public Dictionary<string, string> Headers
	{
		get => (Dictionary<string, string>) GetValue(HeadersProperty);
		set => SetValue(HeadersProperty, value);
	}

	#endregion // DependencyProperty Headers

	#endregion

	public override void Load(IWebViewDelegate renderer)
	{
		if (renderer is IHybridWebViewDelegate hybridRenderer)
			hybridRenderer.LoadUrl(Url.ToString(), Headers);
	}
}