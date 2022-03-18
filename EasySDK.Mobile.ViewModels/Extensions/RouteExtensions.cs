using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class RouteExtensions
{
	#region Nested classes

	class ViewModelFactory<TPage, TViewModel> : RouteFactory
		where TPage : Page
	{
		#region Public methods

		public override Element GetOrCreate()
		{
			try
			{
				if (Application.Current is FormsApp app
				    && app.ServiceProvider.GetService<TPage>() is { } page)
					return page;

				page = Activator.CreateInstance<TPage>();

				if (page.BindingContext != null)
					return page;

				if (Application.Current is FormsApp appForms
				    && appForms.ServiceProvider.GetService<TViewModel>() is { } viewModel)
					page.BindingContext = viewModel;

				return page;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		#endregion
	}

	#endregion

	#region Public methods

	public static void RegisterRoute<TPage, TViewModel>(this IServiceCollection services)
		where TPage: Page
		where TViewModel: class
	{
		services.AddTransient<TViewModel>();

		var typePage = typeof(TPage);

		var routeName = typePage.Name;

		Routing.UnRegisterRoute(routeName);
		Routing.RegisterRoute(routeName, new ViewModelFactory<TPage, TViewModel>());
	}

	#endregion
}