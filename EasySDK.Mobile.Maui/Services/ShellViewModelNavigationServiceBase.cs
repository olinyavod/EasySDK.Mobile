using System.Diagnostics;
using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Maui.Services;

public abstract class ShellViewModelNavigationServiceBase : IViewModelNavigationService
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

	public Task GoToBackAsync(bool animate = true) => GetShell().GoToAsync("..", animate);

	#endregion

	#region Protected methods

	protected abstract Shell GetShell();

	#endregion

	#region Private methods

	private async Task GoToAsync(string route, object? parameter, bool animate, bool removeCurrent)
	{
		var currentShell = GetShell();
		var currentPage = currentShell.CurrentPage;
		var isModal = currentShell.Navigation.ModalStack.Contains(currentPage);

		try
		{
			if (isModal)
				_ = currentShell.Navigation.PopModalAsync(false);

			if (parameter == null)
			{
				await currentShell.GoToAsync(route, animate);
				return;
			}

			await currentShell.GoToAsync(route, animate, new Dictionary<string, object>
			{
				{"Parameter", parameter}
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine("[{0}] Navigation error: {1}.", this, ex);
			throw;
		}
		finally
		{
			if (removeCurrent && !isModal)
				currentShell.Navigation.RemovePage(currentPage);
		}
	}

	#endregion
}