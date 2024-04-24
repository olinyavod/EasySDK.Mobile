using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using EasySDK.Mobile.Forms.Extensions;
using EasySDK.Mobile.Forms.Services;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels;
using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Input;
using EasySDK.Mobile.ViewModels.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms;

public abstract class LoginViewModelBase<TAuthService, TLoginForm> : DataViewModelBase, ILoginViewModel 
	where TLoginForm: class, ILoginForm, new()
	where TAuthService : IAuthService<TLoginForm>
{
	#region Private fields

	private readonly IUserDialogs _dialogs;
	private readonly IUserThemeStorage _userThemeStorage;

	private string?      _login;
	private string?      _password;
	private bool         _invalidLogin;
	private string       _title = ViewModels.Properties.Resources.AuthorizationTitle;
	private ImageSource? _logoImageSource;

	#endregion

	#region Properties

	protected ILogger Log { get; }

	public string Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}

	public ImageSource? LogoImageSource
	{
		get => _logoImageSource;
		set => SetProperty(ref _logoImageSource, value);
	}

	public string? Login
	{
		get => _login;
		set => SetProperty(ref _login, value, LoginOnChanged);
	}

	public string? Password
	{
		get => _password;
		set => SetProperty(ref _password, value, PasswordOnChanged);
	}
	
	#endregion

	#region ctor

	protected LoginViewModelBase
	(
		IServiceScopeFactory scopeFactory,
		IUserDialogs dialogs,
		IUserThemeStorage userThemeStorage,
		ILogger logger,
		IValidator validator
	) : base(scopeFactory, validator)
	{
		_dialogs = dialogs;
		_userThemeStorage = userThemeStorage;
		Log = logger;

		var application = Application.Current;

		application.RequestedThemeChanged += ApplicationOnRequestedThemeChanged;

		LogoImageSource = GetLogoSource(application.RequestedTheme);

		ChangeThemeCommand = new Command(OnChangeTheme);
		SignInCommand = new AsyncCommand(OnSignIn);
	}

	#endregion

	#region Protected methods

	protected abstract ImageSource GetLogoSource(OSAppTheme theme);

	protected virtual Task<TLoginForm> CreateForm(IServiceProvider scope, TAuthService authService) => Task.FromResult(new TLoginForm()
	{
		Login = Login,
		Password = Password
	});

	protected abstract Task SignInOnSuccess(IServiceProvider scope, TLoginForm form, string token);

	protected virtual void ShowInvalidAuthData(IResponse response)
	{
		SetError(ViewModels.Properties.Resources.InvalidUserOrPassword, nameof(Login));
	}

	protected virtual void SetErrors(IResponse response)
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

	protected virtual void ShowSignInErrors(IResponse response)
	{
		_dialogs.ShowErrorMessage(ViewModels.Properties.Resources.FailedSignInMessage);
	}

	#endregion

	#region Private methods

	private void ApplicationOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		LogoImageSource = GetLogoSource(e.RequestedTheme);
	}
	
	private void LoginOnChanged()
	{
		_invalidLogin = false;
	}

	private void PasswordOnChanged()
	{
		if(_invalidLogin)
			ClearErrors(nameof(Login));

		_invalidLogin = false;
	}

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

			using var loadingDlg = _dialogs.Loading(ViewModels.Properties.Resources.Authorization);
			await using var scope = CreateAsyncScope();
			var authService = scope.ServiceProvider.GetService<TAuthService>()!;
			
			var form = await CreateForm(scope.ServiceProvider, authService);
			var response = await authService.LoginAsync(form);
			
			switch (response.ErrorCode)
			{
				case ResponseErrorCodes.Ok:
					Login = string.Empty;
					Password = string.Empty;
					await SignInOnSuccess(scope.ServiceProvider, form, response.Result);
					break;

				case ResponseErrorCodes.DataError:
					SetErrors(response);
					break;

				case ResponseErrorCodes.NotFound:
					ShowInvalidAuthData(response);
					_invalidLogin = true;
					break;

				default:
					ShowSignInErrors(response);
					break;
			}
		}
		catch (Exception ex)
		{
			Log.LogError(ex, "Login error.");
			_dialogs.ShowErrorMessage(ViewModels.Properties.Resources.FailedSignInMessage);
		}
	}

	#endregion
}