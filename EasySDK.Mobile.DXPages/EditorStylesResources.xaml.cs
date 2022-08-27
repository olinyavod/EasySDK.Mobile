using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.DXPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditorStylesResources
	{
		public EditorStylesResources()
		{
			InitializeComponent();
		}

		private void Init()
		{
			DevExpress.XamarinForms.Editors.Initializer.Init();
		}
	}
}