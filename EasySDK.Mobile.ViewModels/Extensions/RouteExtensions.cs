using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class RouteExtensions
{
	#region Nested classes

	class ViewModelFactory<TPage, TViewModel> : RouteFactory
		where TPage : Page
	{
		#region Private fields

		private readonly Action<Page> _initialize;

		#endregion

		#region ctor

		public ViewModelFactory(Action<Page> initialize)
		{
			_initialize = initialize;
		}

		#endregion

		#region Public methods

		public override Element GetOrCreate()
		{
			if (Application.Current is not FormsApp {ServiceProvider: { } serviceProvider})
				return null;

			var log = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<TPage>();

			try
			{
				var page = CreatePage(serviceProvider);

				_initialize?.Invoke(page);

				return page;
			}
			catch (Exception ex)
			{
				log?.LogError(ex, "Create page error.");
				return null;
			}
		}

		#endregion

		#region Private methods

		private Page CreatePage(IServiceProvider serviceProvider)
		{
			if (serviceProvider.GetService<TPage>() is { } page)
				return page;

			page = Activator.CreateInstance<TPage>();

			if (page.BindingContext != null)
				return page;

			if (Application.Current is FormsApp appForms
			    && appForms.ServiceProvider.GetService<TViewModel>() is { } viewModel)
				page.BindingContext = viewModel;

			return page;
		}

		#endregion
	}

	#endregion

	#region Public methods

	public static void RegisterRoute<TPage, TViewModel>(this IServiceCollection services, Action<Page> initialize = null)
		where TPage: Page
		where TViewModel: class
	{
		services.AddTransient<TViewModel>();

		var typePage = typeof(TPage);

		var routeName = typePage.Name;

		Routing.UnRegisterRoute(routeName);
		Routing.RegisterRoute(routeName, new ViewModelFactory<TPage, TViewModel>(initialize));
	}

	#endregion
}