using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IViewModelNavigationService
{
	#region Methods

	Task GoToAsync<TViewModel>(object? parameter = null, bool animate = true, bool removeCurrent = false) where TViewModel: class;
	
	Task GoToRootAsync<TViewModel>(object? parameter = null, bool animate = true, bool removeCurrent = false) where TViewModel: class;

	Task GoToBackAsync(bool animate = true);

	#endregion
}