using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", "EasySDK.Mobile.ViewModels")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", "EasySDK.Mobile.ViewModels.Pages")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", "EasySDK.Mobile.ViewModels.Extensions")]

namespace EasySDK.Mobile.ViewModels;

public abstract class FormsApp : Application
{
	#region Properties

	public IServiceProvider ServiceProvider { get; }

	#endregion

	#region ctor

	protected FormsApp(Action<IServiceCollection> onPlatform)
	{
		ServiceCollection services = new();

		onPlatform?.Invoke(services);
		
		ServiceProvider = RegisterModules(services);
	}

	#endregion

	#region Protected methods

	protected abstract IServiceProvider RegisterModules(IServiceCollection services);

	#endregion
}