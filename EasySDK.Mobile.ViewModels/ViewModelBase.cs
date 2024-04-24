using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.ViewModels;

public abstract class ViewModelBase : NotifyPropertyChangedBase
{
	private readonly IServiceScopeFactory _scopeFactory;

	protected ViewModelBase(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
	}

	#region Protected methods

	protected IServiceScope CreateScope() => _scopeFactory.CreateScope();

	protected AsyncServiceScope CreateAsyncScope() => _scopeFactory.CreateAsyncScope();

	#endregion
}