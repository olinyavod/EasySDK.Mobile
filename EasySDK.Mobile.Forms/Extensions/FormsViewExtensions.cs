using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using EasySDK.Mobile.Forms.Services;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Extensions;

public static class FormsViewExtensions
{
	#region Properties

	public static Color ErrorColor { get; set; } = Color.Red;

	public static Color DoneColor { get; set; } = Color.FromHex("149714");

	#endregion

	#region Public methods

	public static void SwitchAppTheme(this IUserThemeStorage settings)
	{
		var app = Application.Current;

		app.UserAppTheme = app.RequestedTheme == OSAppTheme.Dark
			? OSAppTheme.Light
			: OSAppTheme.Dark;

		settings.UserAppTheme = app.UserAppTheme;
	}

	public static void ShowErrorMessage(this IUserDialogs dialogs, string message, ToastPosition position = ToastPosition.Bottom)
	{
		var errorColor = ErrorColor;

		dialogs.Toast(
			new ToastConfig(message)
			{
				Position = position,
				Icon = "error_white_18.png",
				BackgroundColor = errorColor,
				MessageTextColor = Color.White
			});
	}

	public static void ShowSuccessMessage(this IUserDialogs dialogs, string message, ToastPosition position = ToastPosition.Bottom)
	{
		var doneColor = DoneColor;

		dialogs.Toast(new ToastConfig(message)
		{
			Position = position,
			BackgroundColor = doneColor,
			MessageTextColor = Color.White,
			Icon = "circle_check_white_18.png"
		});
	}

	public static int? GetAndroidStyleId(this Application app)
	{
		var androidDialogStyles = DependencyService.Resolve<IServiceProvider>().GetService<IAndroidDialogStyles>();

		return app.RequestedTheme == OSAppTheme.Dark
			? androidDialogStyles?.AlertDarkStyleId ?? 0
			: androidDialogStyles?.AlertLightStyleId ?? 0;
	}

	public static Task<bool> ShowConfirmAsync
	(
		this IUserDialogs dialogs,
		string message,
		string title = null,
		string yes = null,
		string no = null
	)
	{
		var app = Application.Current;
		var androidDialogStyles = DependencyService.Resolve<IServiceProvider>().GetService<IAndroidDialogStyles>();
		
		return dialogs.ConfirmAsync(new ConfirmConfig
		{
			Message    = message,
			Title      = title,
			CancelText = no ?? ViewModels.Properties.Resources.No,
			OkText     = yes ?? ViewModels.Properties.Resources.Yes,
			AndroidStyleId = app.RequestedTheme == OSAppTheme.Dark
				? androidDialogStyles?.AlertDarkStyleId ?? 0
				: androidDialogStyles?.AlertLightStyleId ?? 0
		});
	}

	public static Task ShowAlertAsync
	(
		this IUserDialogs dialog,
		string message,
		string title = null,
		string ok = null
	)
	{
		var app = Application.Current;
		var androidDialogStyles = DependencyService.Resolve<IServiceProvider>().GetService<IAndroidDialogStyles>();

		return dialog.AlertAsync(new AlertConfig
		{
			OkText  = ok ?? ViewModels.Properties.Resources.Ok,
			Message = message,
			Title   = title,
			AndroidStyleId = app.RequestedTheme == OSAppTheme.Dark
				? androidDialogStyles?.AlertDarkStyleId ?? 0
				: androidDialogStyles?.AlertLightStyleId ?? 0
		});
	}

	public static async Task<bool> RequestPermissionIfDeny<TPermission>
	(
		this IUserDialogs dialogs,
		string rationaleMessage,
		string promptGoToSettings = null,
		string title = null
	)
		where TPermission : Xamarin.Essentials.Permissions.BasePermission, new()
	{
		var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<TPermission>();

		switch (status)
		{
			case PermissionStatus.Granted:
				return true;

			case PermissionStatus.Disabled:
				return false;

			case PermissionStatus.Denied when Device.RuntimePlatform == Device.iOS:
				await dialogs.ShowAlertAsync(promptGoToSettings, title ?? ViewModels.Properties.Resources.RequestPermissions);
				return false;
		}

		if (Xamarin.Essentials.Permissions.ShouldShowRationale<TPermission>())
			await dialogs.ShowAlertAsync(rationaleMessage, title ?? ViewModels.Properties.Resources.RequestPermissions);

		status = await Xamarin.Essentials.Permissions.RequestAsync<TPermission>();

		return status == PermissionStatus.Granted;

	}

	public static IServiceScope CreateScope(this BindableObject _)
	{
		var app = (FormsApp) Application.Current;

		return app.ServiceProvider.CreateScope();
	}

	#endregion
}