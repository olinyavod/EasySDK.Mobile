using Xamarin.Forms;
using static EasySDK.Mobile.ViewModels.Themes.DefaultColorThemeKeys;

namespace EasySDK.Mobile.ViewModels.Themes
{
	public class ThemedShell : Shell
	{
		public ThemedShell()
		{
			SetDynamicResource(BackgroundColorProperty, nameof(PageBackgroundColor));
			SetDynamicResource(TitleColorProperty, nameof(TextColor));
			SetDynamicResource(ForegroundColorProperty, nameof(TextColor));
			SetDynamicResource(DisabledColorProperty, nameof(TextColor));
			SetDynamicResource(UnselectedColorProperty, nameof(UnselectedTextColor));
			SetDynamicResource(FlyoutBackgroundColorProperty, nameof(PageBackgroundColor));
		}
	}
}
