using System.Threading.Tasks;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IAuthService<in TLoginFrom>
	where TLoginFrom : class, ILoginForm
{
	#region Methods

	Task<IResponse<string>> LoginAsync(TLoginFrom form);

	Task<IResponse<bool>> LogoutAsync();

	#endregion
}