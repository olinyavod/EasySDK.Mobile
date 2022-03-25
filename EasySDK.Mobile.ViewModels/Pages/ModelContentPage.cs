using System;
using Microsoft.Extensions.Logging;
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
		if (Application.Current is not FormsApp {ServiceProvider: { } serviceProvider})
			return;

		try
		{
			ViewModel = serviceProvider.GetService<TViewModel>();
			BindingContext = ViewModel;
		}
		catch (Exception ex)
		{
			var log = serviceProvider
				.GetService<ILoggerFactory>()
				.CreateLogger<ModelContentPage<TViewModel>>();

			log.LogError(ex, "Initialize page error.");
		}
	}

	#endregion

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (ViewModel is ISupportAppearing appearing)
			appearing.OnAppearing();
	}
}