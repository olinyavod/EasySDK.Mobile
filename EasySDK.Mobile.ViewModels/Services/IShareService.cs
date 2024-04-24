using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IShareService
{
	Task ShareFileAsync(string filePath, string mimeType);
}