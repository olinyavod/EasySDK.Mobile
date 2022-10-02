#nullable enable

using System.Linq;
using System.Threading.Tasks;
using Android.Accounts;
using Android.Content;
using Android.OS;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Android.Services;

public class AndroidDeviceAccountService : IDeviceAccountService
{
	#region Private fields

	private readonly Context _context;
	private readonly string _accountType;
	private readonly string _authTokenType;

	#endregion

	#region ctor

	public AndroidDeviceAccountService(Context context, string accountType, string authTokenType)
	{
		_context = context;
		_accountType = accountType;
		_authTokenType = authTokenType;
	}

	#endregion

	#region Public methods

	public bool AddAccount(string login, string password, string token)
	{
		using var am = AccountManager.Get(_context);
		using var account = new Account(login, _accountType);

		if (am?.AddAccountExplicitly(account, password, null) is true)
		{
			am.SetAuthToken(account, _authTokenType, token);
			return true;
		}

		return false;
	}

	public string? GetAccountName()
	{
		using var am = AccountManager.Get(_context);
		using var account = GetAccount(am);

		return account?.Name;
	}

	public bool RemoveAccount()
	{
		using var am = AccountManager.Get(_context);
		using var account = GetAccount(am);
		
		return am?.RemoveAccountExplicitly(account) ?? false;
	}

	public bool InvalidAuthToken()
	{
		using var am = AccountManager.Get(_context);

		am?.InvalidateAuthToken(_accountType, _authTokenType);

		return true;
	}

	public Task<string?> TryGetAuthTokenAsync() => Task.Run(() =>
	{
		using var am = AccountManager.Get(_context);
		using var account = GetAccount(am);
		
		return am?.BlockingGetAuthToken(account, _authTokenType, true);
	});
	
	#endregion

	#region Private methods

	private Account? GetAccount(AccountManager? manager) => manager
		?.GetAccountsByTypeForPackage(_accountType, _context.PackageName)
		.FirstOrDefault();

	#endregion
}