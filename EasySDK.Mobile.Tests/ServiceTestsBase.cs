using EasySDK.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.Tests;

public class ServiceTestsBase<TService>
{
	#region Private fields

	private IServiceScope? _scope;
	private TService? _service;

	#endregion

	#region Properties

	protected IServiceProvider ServiceProvider => _scope!.ServiceProvider;

	protected TService Service => _service!;

	#endregion

	#region Public methods

	[SetUp]
	public void Setup()
	{
		_scope = IPlatformApplication.Current!.ServiceProvider!.CreateScope();

		_service = ServiceProvider.GetService<TService>();
	}

	[TearDown]
	public void Clear()
	{
		_scope?.Dispose();
	}

	#endregion
}