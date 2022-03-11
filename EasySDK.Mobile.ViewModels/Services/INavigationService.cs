using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public interface INavigationService
{
	Task GoToAsync(ShellNavigationState state, bool animate = true);
}