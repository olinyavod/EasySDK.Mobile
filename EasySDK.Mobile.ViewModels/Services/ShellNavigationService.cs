using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public class ShellNavigationService : INavigationService
{
	public TViewModel GetCurrentViewModel<TViewModel>() where TViewModel : class => Shell.Current?.CurrentPage?.BindingContext as TViewModel;

	public Task GoToAsync(ShellNavigationState state, bool animate = true) => Shell.Current.GoToAsync(state, animate);
}