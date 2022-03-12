using System;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Controls;

public class HybridWebView : WebView
{
    #region Events

    public event EventHandler<InvokeEventArgs> Invoke;
    
    #endregion

    #region Public methods

    public void InvokeAction(string data)
    {
        Invoke?.Invoke(this, new InvokeEventArgs(data));
    }

    #endregion
}