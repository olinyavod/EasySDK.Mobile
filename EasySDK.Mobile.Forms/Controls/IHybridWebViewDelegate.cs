using System.Collections.Generic;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public interface IHybridWebViewDelegate : IWebViewDelegate
{
	#region Methods

	void LoadUrl(string url, IDictionary<string, string> headers);

	#endregion
}