using System;
using EasySDK.Mobile.Forms.Themes;
using EasySDK.Mobile.ViewModels.Pages;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.Forms.Pages;

public abstract class ModelContentPage<TViewModel> : ContentPage
{
	#region Properties

	public TViewModel ViewModel { get; }

	#endregion

	#region ctor

	protected ModelContentPage()
	{
		SetDynamicResource(BackgroundColorProperty, nameof(DefaultColorThemeKeys.PageBackgroundColor));

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

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		if(ViewModel is ISupportDisappearing disappearing)
			disappearing.OnDisappearing();

	}
}