using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Services;
using Xamarin.Essentials;

namespace EasySDK.Mobile.Forms.Services;

public class XamarinClipboardService : IClipboardService
{
	public Task SetTextAsync(string text) => Clipboard.SetTextAsync(text);
}