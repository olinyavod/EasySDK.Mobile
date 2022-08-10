using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public interface INavigationService
{
	TViewModel GetCurrentViewModel<TViewModel>() where TViewModel : class;

	Task GoToAsync(ShellNavigationState state, bool animate = true);
}