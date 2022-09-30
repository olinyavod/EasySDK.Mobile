#nullable enable

using System.Linq;
using System.Threading.Tasks;
using Android.Accounts;
using Android.Content;
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

	public bool AddAccount(string login, string password)
	{
		using var am = AccountManager.Get(_context);
		using var account = new Account(login, _accountType);

		return am?.AddAccountExplicitly(account, password, null) ?? false;
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

	public Task<string?> TryGetAccountTokenAsync() => Task.Run(() =>
	{
		using var am = AccountManager.Get(_context);
		using var account = GetAccount(am);

		return am?.BlockingGetAuthToken(account, _accountType, true);
	});
	
	#endregion

	#region Private methods

	private Account? GetAccount(AccountManager? manager) => manager
		?.GetAccountsByTypeForPackage(_accountType, _context.PackageName)
		.FirstOrDefault();

	#endregion
}