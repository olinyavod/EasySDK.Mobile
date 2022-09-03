#nullable enable

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Themes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DefaultColorsResources
	{
		private ResourceDictionary? _oldColors;
		private ColorPaletteBase _lightPalette = new DefaultLightPalette();
		private ColorPaletteBase _darkPalette = new DefaultDarkPalette();

		public ColorPaletteBase LightPalette
		{
			get => _lightPalette;
			set
			{
				if(_lightPalette == value)
					return;

				_lightPalette = value;
				var app = Application.Current;
				CurrentOnRequestedThemeChanged(app, new AppThemeChangedEventArgs(app.UserAppTheme));
			}
		}

		public ColorPaletteBase DarkPalette
		{
			get => _darkPalette;
			set
			{
				if(_darkPalette == value)
					return;

				_darkPalette = value;
				var app = Application.Current;
				CurrentOnRequestedThemeChanged(app, new AppThemeChangedEventArgs(app.UserAppTheme));
			}
		}


		public DefaultColorsResources()
		{
			InitializeComponent();

			var application = Application.Current;

			application.RequestedThemeChanged += CurrentOnRequestedThemeChanged;

			CurrentOnRequestedThemeChanged(application, new AppThemeChangedEventArgs(application.UserAppTheme));
		}
		
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