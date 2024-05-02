using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IViewModelNavigationService
{
	Task GoToAsync<TViewModel>(object? parameter = null, bool animate = true) where TViewModel: class;
	
	Task GoToRootAsync<TViewModel>(object? parameter = null, bool animate = true) where TViewModel: class;

	Task GotToBackAsync(bool animate = true);
}