using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Services;

public interface IUserThemeStorage
{
	OSAppTheme? UserAppTheme { get; set; }
}