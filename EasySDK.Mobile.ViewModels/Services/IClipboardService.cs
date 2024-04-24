using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IClipboardService
{
	Task SetTextAsync(string text);
}