﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
	#region Protected methods

	protected bool SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		onChanged?.Invoke();
		OnPropertyChanged(propertyName);
		return true;
	}

	protected void OnPropertiesChanged(params string[] properties)
	{
		foreach (var p in properties ?? Array.Empty<string>())
			OnPropertyChanged(p);
	}

	protected IServiceScope CreateScope()
	{
		var app = (FormsApp) Application.Current;

		return app.ServiceProvider.CreateScope();
	}

	protected AsyncServiceScope CreateAsyncScope()
	{
		var app = (FormsApp) Application.Current;

		return app.ServiceProvider.CreateAsyncScope();
	}

	#endregion

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
	#endregion
}