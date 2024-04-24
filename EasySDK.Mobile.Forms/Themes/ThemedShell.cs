using Xamarin.Forms;
using static EasySDK.Mobile.Forms.Themes.DefaultColorThemeKeys;

namespace EasySDK.Mobile.Forms.Themes
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
