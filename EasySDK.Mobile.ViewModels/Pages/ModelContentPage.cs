using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Pages;

public abstract class ModelContentPage<TViewModel> : ContentPage
{
	#region Properties

	public TViewModel ViewModel { get; }

	#endregion

	#region ctor

	protected ModelContentPage()
	{
		if (Application.Current is FormsApp app)
			ViewModel = app.ServiceProvider.GetService<TViewModel>();

		BindingContext = ViewModel;
	}

	#endregion

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (ViewModel is ISupportAppearing appearing)
			appearing.OnAppearing();
	}
}