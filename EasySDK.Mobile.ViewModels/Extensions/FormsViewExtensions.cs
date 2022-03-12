using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Extensions;

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
			Message = message,
			Title = title,
			CancelText = no ?? Properties.Resources.No,
			OkText = yes ?? Properties.Resources.Yes,
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
			OkText = ok ?? Properties.Resources.Ok,
			Message = message,
			Title = title,
			AndroidStyleId = app.RequestedTheme == OSAppTheme.Dark
				? androidDialogStyles?.AlertDarkStyleId ?? 0
				: androidDialogStyles?.AlertLightStyleId ?? 0
		});
	}

	#endregion
}