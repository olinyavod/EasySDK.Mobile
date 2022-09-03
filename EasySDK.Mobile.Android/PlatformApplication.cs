using System;
using Android.App;
using Android.Runtime;
using EasySDK.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.Android;

public abstract class PlatformApplication : Application, IPlatformApplication
{
	#region Private fields

	private ServiceProvider? _serviceProvider;

	#endregion

	#region Properties

	public IServiceProvider? ServiceProvider => _serviceProvider;

	#endregion

	#region ctor

	protected PlatformApplication(IntPtr javaReference, JniHandleOwnership transfer)
		: base(javaReference, transfer)
	{
		IPlatformApplication.Current = this;
	}

	#endregion

	#region Public methods

	public override void OnCreate()
	{
		base.OnCreate();
		
		var services = new ServiceCollection();

		RegisterIoC(services);

		_serviceProvider = services.BuildServiceProvider();
	}

	public override void OnTerminate()
	{
		base.OnTerminate();
		_serviceProvider?.Dispose();

		
	}

	#endregion

	#region Protected methods

	protected abstract void RegisterIoC(IServiceCollection services);

	#endregion
}