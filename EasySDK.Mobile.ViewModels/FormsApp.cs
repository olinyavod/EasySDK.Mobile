using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public abstract class FormsApp : Application
{
	internal const string BaseNamespace = $"{nameof(EasySDK)}.{nameof(Mobile)}.{nameof(ViewModels)}";

	#region Properties

	public IServiceProvider ServiceProvider { get; }

	#endregion

	#region ctor

	protected FormsApp(Action<IServiceCollection> onPlatform)
	{
		ServiceCollection services = new();

		onPlatform?.Invoke(services);
		
		ServiceProvider = RegisterModules(services);

		DependencyService.RegisterSingleton(ServiceProvider);
	}

	#endregion

	#region Protected methods

	protected abstract IServiceProvider RegisterModules(IServiceCollection services);

	#endregion
}