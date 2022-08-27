using System;
using FluentValidation;
using System.Windows.Input;
using Acr.UserDialogs;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;
using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Input;

namespace EasySDK.Mobile.ViewModels;

public abstract class LoginViewModelBase<TLoginForm> : DataViewModelBase, ILoginForm
	where TLoginForm: class, ILoginForm, new()
{
	#region Private fields

	private readonly IUserDialogs _dialogs;
	private readonly IUserThemeStorage _userThemeStorage;
	private readonly IAuthService<TLoginForm> _authService;

	private string _login;
	private string _password;

	#endregion

	#region Properties

	protected ILogger Log { get; }

	public string Login
	{
		get => _login;
		set => SetProperty(ref _login, value);
	}

	public string Password
	{
		get => _password;
		set => SetProperty(ref _password, value);
	}
	
	#endregion

	#region ctor

	protected LoginViewModelBase
	(
		IUserDialogs dialogs,
		IUserThemeStorage userThemeStorage,
		ILogger logger,
		IAuthService<TLoginForm> authService,
		IValidator validator
	) : base(validator)
	{
		_dialogs = dialogs;
		_userThemeStorage = userThemeStorage;
		_authService = authService;
		Log = logger;

		ChangeThemeCommand = new Command(OnChangeTheme);
		SignInCommand = new AsyncCommand(OnSignIn);
	}
	
	#endregion

	#region Protected methods

	protected virtual TLoginForm CreateForm() => new()
	{
		Login = Login,
		Password = Password
	};

	protected abstract Task SignInOnSuccess(TLoginForm form, string token);

	#endregion

	#region ChangeThemeCommand

	public ICommand ChangeThemeCommand { get; }
	
	private void OnChangeTheme() => _userThemeStorage.SwitchAppTheme();

	#endregion

	#region SingInCommand

	public ICommand SignInCommand { get; }

	private async Task OnSignIn()
	{
		try
		{
			if(!await ValidateAsync())
				return;

			using var loadingDlg = _dialogs.Loading(Properties.Resources.Authorization);

			var form = CreateForm();
			var response = await _authService.LoginAsync(form);
			
			switch (response.ErrorCode)
			{
				case ResponseErrorCodes.Ok:
					Login = string.Empty;
					Password = string.Empty;
					await SignInOnSuccess(form, response.Result);
					break;

				case ResponseErrorCodes.DataError:
					SetErrors(response);
					break;

				case ResponseErrorCodes.NotFound:
					SetError(Properties.Resources.InvalidUserOrPassword, nameof(Login));
					break;

				default:
					_dialogs.ShowErrorMessage(Properties.Resources.FailedSignInMessage);
					break;
			}

		}
		catch (Exception ex)
		{
			Log.LogError(ex, "Login error.");
			_dialogs.ShowErrorMessage(Properties.Resources.FailedSignInMessage);
		}
	}

	#endregion

	protected void SetErrors(IResponse response)
	{
		if (response.ErrorMessages is not { } errors)
			return;

		var map = typeof(TLoginForm).GetPropertiesMap();

		foreach (var error in errors)
		{
			if (!map.TryGetValue(error.Key, out var propertyName))
				propertyName = error.Key;

			SetErrors(error.Value, propertyName);
		}
	}
}