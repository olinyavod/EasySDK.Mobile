using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
	private readonly IServiceScopeFactory _scopeFactory;

	protected ViewModelBase(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
	}

	#region Protected methods

	protected bool SetProperty<T>(ref T backingStore, T value, Action? onChanged = null, [CallerMemberName] string? propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		onChanged?.Invoke();
		OnPropertyChanged(propertyName!);
		return true;
	}

	protected void OnPropertiesChanged(params string[]? properties)
	{
		foreach (var p in properties ?? Array.Empty<string>())
			OnPropertyChanged(p);
	}

	protected IServiceScope CreateScope() => _scopeFactory.CreateScope();

	protected AsyncServiceScope CreateAsyncScope() => _scopeFactory.CreateAsyncScope();

	#endregion

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
	#endregion
}