using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Maui.Services;

public class MauiShareService : IShareService
{
	public Task ShareFileAsync(string filePath, string mimeType)
	{
		return Share.RequestAsync(new ShareFileRequest(Path.GetFileName(filePath), new ShareFile(filePath, mimeType)));
	}
}