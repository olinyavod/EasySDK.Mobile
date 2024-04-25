﻿using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Maui.Services;

public class ShellViewModelNavigationService : IViewModelNavigationService
{
	#region Public methods

	public Task GoToAsync<TViewModel>(object? parameter = null, bool animate = true) where TViewModel : class
	{
		var viewName = typeof(TViewModel).GetViewKey();

		return GoToAsync(viewName, parameter, animate);
	}

	public Task GoToRootAsync<TViewModel>(object? parameter = null, bool animate = true) where TViewModel : class
	{
		var viewName = typeof(TViewModel).GetViewKey();

		return GoToAsync($"//{viewName}", parameter, animate);
	}

	#endregion

	#region Private methods

	private Task GoToAsync(string route, object? parameter, bool animate)
	{
		if (parameter == null)
			return Shell.Current.GoToAsync(route, animate);

		return Shell.Current.GoToAsync(route, animate,  new Dictionary<string, object>
		{
			{"Parameter", parameter}
		});
	}

	#endregion
}