using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.DXPages.Themes
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataFormResources
	{
		public DataFormResources()
		{
			InitializeComponent();

			DevExpress.XamarinForms.DataForm.Initializer.Init();
		}
	}
}