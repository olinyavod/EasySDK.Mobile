using System.Threading.Tasks;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.ViewModels.Managers;

public interface IAuthManager<TLoginFrom> : IAuthService<TLoginFrom>
	where TLoginFrom : class, ILoginForm
{
	bool CheckIsLogin();

	Task<TLoginFrom> CreateFormAsync(string login, string password);
}