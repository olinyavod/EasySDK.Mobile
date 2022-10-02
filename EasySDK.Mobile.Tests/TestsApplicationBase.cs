using EasySDK.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.Tests;

public abstract class TestsApplicationBase : IPlatformApplication
{
	#region Private fields

	private ServiceProvider? _serviceProvider;

	#endregion

	public IServiceProvider? ServiceProvider => _serviceProvider;

	[OneTimeSetUp]
	public void GlobalSetUp()
	{
		IPlatformApplication.Current = this;
		ServiceCollection services = new();

		ConfigureIoC(services);
		
		_serviceProvider = services.BuildServiceProvider();
	}

	[OneTimeTearDown]
	public void GlobalClear()
	{
		_serviceProvider?.Dispose();
	}

	protected abstract void ConfigureIoC(IServiceCollection service);
}