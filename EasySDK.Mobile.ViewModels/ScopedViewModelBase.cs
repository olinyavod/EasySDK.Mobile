using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.ViewModels;

public abstract class ScopedViewModelBase : ViewModelBase
{
	private readonly IServiceScopeFactory _scopeFactory;

	protected ScopedViewModelBase(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
	}

	#region Protected methods

	protected IServiceScope CreateScope() => _scopeFactory.CreateScope();

	protected AsyncServiceScope CreateAsyncScope() => _scopeFactory.CreateAsyncScope();

	#endregion
}