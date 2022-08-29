#nullable enable

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Themes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DefaultColorsResources
	{
		public ColorPaletteBase LightPalette { get; set; } = new DefaultLightPalette();

		public ColorPaletteBase DarkPalette { get; set; } = new DefaultDarkPalette();


		public DefaultColorsResources()
		{
			InitializeComponent();

			var application = Application.Current;

			application.RequestedThemeChanged += CurrentOnRequestedThemeChanged;

			CurrentOnRequestedThemeChanged(application, new AppThemeChangedEventArgs(application.UserAppTheme));
		}

		private ResourceDictionary? _oldColors;

		private void CurrentOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			if (_oldColors != null)
				MergedDictionaries.Remove(_oldColors);

			_oldColors = e.RequestedTheme == OSAppTheme.Dark
				? DarkPalette
				: LightPalette;

			MergedDictionaries.Add(_oldColors);
		}
	}

	public class ThemeKeyExtensionBase<TKey> : IMarkupExtension
	{
		public TKey Key { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return Key.ToString();
		}
	}

	public class DefaultColorThemeKeyExtension : ThemeKeyExtensionBase<DefaultColorThemeKeys>
	{
	}
}