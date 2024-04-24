using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Maui.Services;

public class MauiClipboardService : IClipboardService
{
	public Task SetTextAsync(string text) => Clipboard.SetTextAsync(text);
}