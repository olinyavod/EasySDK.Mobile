using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Maui.Services;

public class ShellViewModelNavigationService : IViewModelNavigationService
{
	#region Public methods

	public Task GoToAsync<TViewModel>(object? parameter = null, bool animate = true, bool removeCurrent = false) where TViewModel : class
	{
		var viewName = typeof(TViewModel).GetViewKey();

		return GoToAsync(viewName, parameter, animate, removeCurrent);
	}

	public Task GoToRootAsync<TViewModel>(object? parameter = null, bool animate = true, bool removeCurrent = false) where TViewModel : class
	{
		var viewName = typeof(TViewModel).GetViewKey();

		return GoToAsync($"//{viewName}", parameter, animate, removeCurrent);
	}

	public Task GoToBackAsync(bool animate = true) => Current.GoToAsync("..", animate);

	#endregion

	#region Private methods

	private Shell Current => Shell.Current;

	private async Task GoToAsync(string route, object? parameter, bool animate, bool removeCurrent)
	{
		var currentShell = Current;
		var currentPage = currentShell.CurrentPage;
		var isModal = currentShell.Navigation.ModalStack.Contains(currentPage);

		try
		{
			if (isModal)
				_ = currentShell.Navigation.PopModalAsync(false);

			if (parameter == null)
			{
				await Shell.Current.GoToAsync(route, animate);
				return;
			}
			
			await Current.GoToAsync(route, animate, new Dictionary<string, object>
			{
				{"Parameter", parameter}
			});
		}
		finally
		{
			if (removeCurrent && !isModal)
				currentShell.Navigation.RemovePage(currentPage);
		}
	}

	#endregion
}