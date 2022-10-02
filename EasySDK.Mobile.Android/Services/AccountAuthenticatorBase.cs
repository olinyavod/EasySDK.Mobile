#nullable enable

using System;
using System.Threading.Tasks;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Services;
using Java.Lang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace EasySDK.Mobile.Android.Services;

public abstract class AccountAuthenticatorBase<TLoginActivity, TAuthService, TLoginForm> : AbstractAccountAuthenticator
	where TLoginActivity : Activity
	where TLoginForm : class, ILoginForm
	where TAuthService : class, IAuthService<TLoginForm>
{
	#region Properties

	protected IServiceProvider ServiceProvider { get; }

	protected ILogger Logger { get; }

	protected Context Context { get; }

	protected abstract string AuthTokenType { get; }

	#endregion

	#region ctor

	protected AccountAuthenticatorBase(Context context, IServiceProvider serviceProvider)
		: base(context)
	{
		Context = context;
		ServiceProvider = serviceProvider;
		Logger = ServiceProvider.GetService<ILoggerFactory>()!.CreateLogger(GetType());
	}

	#endregion


	public override Bundle AddAccount
	(
		AccountAuthenticatorResponse? response,
		string? accountType,
		string? authTokenType,
		string[]? requiredFeatures,
		Bundle? options
	)
	{
		Logger.LogInformation("Try add account type: {0}.", accountType);
		var bundle = new Bundle();
		
		bundle.PutParcelable(AccountManager.KeyIntent, new Intent(Context, typeof(TLoginActivity))
			.PutExtra(AccountManager.KeyAccountAuthenticatorResponse, response));
		return bundle;
	}

	public override Bundle? ConfirmCredentials(AccountAuthenticatorResponse? response, Account? account, Bundle? options)
	{
		Logger.LogInformation("Confirm credentials.");
		return null;
	}

	public override Bundle EditProperties
	(
		AccountAuthenticatorResponse? response,
		string? accountType
	)
	{
		throw new UnsupportedOperationException();
	}

	public override string? GetAuthTokenLabel(string? authTokenType) => null;

	public override Bundle HasFeatures(AccountAuthenticatorResponse? response, Account? account, string[]? features)
	{
		var result = new Bundle();
		result.PutBoolean(AccountManager.KeyBooleanResult, false);
		return result;
	}

	public override Bundle? UpdateCredentials
	(
		AccountAuthenticatorResponse? response,
		Account? account,
		string? authTokenType,
		Bundle? options
	)
	{
		return null;
	}

	public override Bundle GetAuthToken
	(
		AccountAuthenticatorResponse? response,
		Account? account,
		string? authTokenType,
		Bundle? options
	)
	{
		try
		{
			Logger.LogInformation("Get auth token for type: {0}.", authTokenType);

			if (authTokenType != AuthTokenType)
				return CreateErrorResult("Invalid auth token type.");

			using var am = AccountManager.Get(Context)!;
			var password = am.GetPassword(account);

			if (!string.IsNullOrWhiteSpace(password))
			{
				var token = Task.Run(() => GetAuthTokenAsync(account!.Name, password)).Result;

				if (string.IsNullOrWhiteSpace(token))
					return CreateErrorResult("Failed get token.");

				return CreateTokenResult(account!, token!);
			}

			var bundle = new Bundle();
			bundle.PutParcelable(AccountManager.KeyIntent, new Intent(Context, typeof(TLoginActivity))
				.PutExtra(AccountManager.KeyAccountName, account?.Name)
				.PutExtra(AccountManager.KeyAuthtoken, authTokenType)
				.PutExtra(AccountManager.KeyAccountAuthenticatorResponse, response));
			return bundle;
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Get auth token error.");

			return CreateErrorResult("Invalid auth token type.");
		}
	}

	#region Protected methods

	protected abstract Task<TLoginForm> CreateFormAsync(IServiceProvider scope, string login, string password);

	protected virtual async Task<string?> GetAuthTokenAsync(string login, string password)
	{
		await using var scope = ServiceProvider.CreateAsyncScope();
		var form = await CreateFormAsync(scope.ServiceProvider, login, password);
		var authService = scope.ServiceProvider.GetService<TAuthService>()!;

		var response = await authService.LoginAsync(form);

		if (response.HasError)
			return string.Empty;

		return response.Result;
	}

	protected Bundle CreateTokenResult(Account account, string token)
	{
		var result = new Bundle();
		result.PutString(AccountManager.KeyAccountName, account.Name);
		result.PutString(AccountManager.KeyAccountType, account.Type);
		result.PutString(AccountManager.KeyAuthtoken, token);
		return result;
	}

	protected Bundle CreateErrorResult(string message)
	{
		var result = new Bundle();
		result.PutString(AccountManager.KeyErrorMessage, message);
		return result;
	}

	#endregion
}