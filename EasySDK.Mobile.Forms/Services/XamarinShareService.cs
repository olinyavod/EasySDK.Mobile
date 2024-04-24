using System.IO;
using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Services;
using Xamarin.Essentials;

namespace EasySDK.Mobile.Forms.Services;

public class XamarinShareService : IShareService
{
	public Task ShareFileAsync(string filePath, string mimeType)
	{
		return Share.RequestAsync(new ShareFileRequest(Path.GetFileName(filePath), new ShareFile(filePath, mimeType)));
	}
}