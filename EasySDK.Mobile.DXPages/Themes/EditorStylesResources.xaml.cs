using DevExpress.XamarinForms.Core.Themes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.DXPages.Themes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditorStylesResources
	{
		public EditorStylesResources()
		{
			InitializeComponent();

			Application.Current.RequestedThemeChanged += CurrentOnRequestedThemeChanged;
		}

		private void CurrentOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			ThemeManager.ThemeName = e.RequestedTheme == OSAppTheme.Dark ? Theme.Dark : Theme.Light;
		}

		private void Init()
		{
			DevExpress.XamarinForms.Editors.Initializer.Init();
		}
	}
}