using System.Windows.Input;
using EasySDK.Mobile.Models;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public interface ILoginViewModel : ILoginForm
{
	string Title { get; set; }

	ImageSource LogoImageSource { get; set; }
	
	ICommand ChangeThemeCommand { get; }

	ICommand SignInCommand { get; }
}