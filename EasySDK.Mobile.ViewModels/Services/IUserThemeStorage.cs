using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IUserThemeStorage
{
	OSAppTheme? UserAppTheme { get; set; }
}