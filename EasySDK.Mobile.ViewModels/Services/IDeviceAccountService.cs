using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services
{
	public interface IDeviceAccountService
	{
		bool AddAccount(string login, string password, string token);

		string? GetAccountName();

		bool RemoveAccount();

		Task<bool> InvalidAuthToken();

		Task<string?> TryGetAuthTokenAsync();
	}
}
