using EasySDK.Mobile.ViewModels.Extensions;

namespace EasySDK.Mobile.Maui.Extensions;

public static class IoCExtensions
{
	#region Nested classes

	class RouteFactory<TPage, TViewModel> : RouteFactory
		where TPage : Page
		where TViewModel : class
	{
		#region Public methods

		public override Element GetOrCreate()
		{
			throw new NotImplementedException();
		}

		public override Element GetOrCreate(IServiceProvider services)
		{
			var page = services.GetRequiredService<TPage>();

			if (page.BindingContext is not TViewModel)
				page.BindingContext = services.GetRequiredService<TViewModel>();

			Shell.SetTabBarIsVisible(page, false);

			return page;
		}

		#endregion
	}

	#endregion

	#region Public methods

	public static void RegisterRootPage<TPage, TViewModel>(this IServiceCollection services)
		where TPage : Page
		where TViewModel : class
	{
		const string key = "ROOT";

		services.AddTransient<TViewModel>();
		services.AddKeyedTransient<TPage>(key);

		services.AddTransient<TPage>(c =>
		{
			var page = c.GetRequiredKeyedService<TPage>(key);

			if (page.BindingContext is not TViewModel)
				page.BindingContext = c.GetRequiredService<TViewModel>();

			return page;
		});
	}

	public static void RegisterRoute<TPage, TViewModel>(this IServiceCollection services)
		where TPage : Page
		where TViewModel : class
	{
		services.AddTransient<TViewModel>();
		services.AddTransient<TPage>();

		var viewModelKey = typeof(TViewModel).GetViewKey();

		Routing.RegisterRoute(viewModelKey, new RouteFactory<TPage,TViewModel>());
	}

	#endregion
}