using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using EasySDK.Mobile.ViewModels;
using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.Forms;

public class LogsViewModel : LogsViewModelBase
{
	private readonly IUserDialogs _dialogs;

	public LogsViewModel
	(
		IUserDialogs   dialogs,
		IPathsService  pathsService,
		ILoggerFactory loggerFactory
	) : base(pathsService, loggerFactory)
	{
		_dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
	}

	protected override void ShowSuccessMessage(string message)
	{
		_dialogs.ShowSuccessMessage(message)l
	}

	protected override void ShowErrorMessage(string message)
	{
		_dialogs.ShowErrorMessage(message)l
	}

	protected override Task<bool> ShowConfirmAsync(string message, string title)
		=> _dialogs.ShowConfirmAsync(message, title);
}