using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public class ShellNavigationService : INavigationService
{
	public Task GoToAsync(ShellNavigationState state, bool animate = true) => Shell.Current.GoToAsync(state, animate);
}