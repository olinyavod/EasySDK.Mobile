using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using EasySDK.Mobile.Forms.Extensions;
using EasySDK.Mobile.ViewModels;
using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.Forms;

public class LogsViewModel : LogsViewModelBase
{
	#region Private fields

	private readonly IUserDialogs _dialogs;

	#endregion

	#region ctor

	public LogsViewModel
	(
		IShareService shareService,
		IClipboardService clipboardService,	
		IServiceScopeFactory scopeFactory,
		IUserDialogs         dialogs,
		IPathsService        pathsService,
		ILoggerFactory       loggerFactory
	) : base(shareService, clipboardService, scopeFactory, pathsService, loggerFactory)
	{
		_dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
	}

	#endregion

	protected override void ShowSuccessMessage(string message)
	{
		_dialogs.ShowSuccessMessage(message);
	}

	protected override void ShowErrorMessage(string message)
	{
		_dialogs.ShowErrorMessage(message);
	}

	protected override Task<bool> ShowConfirmAsync(string message, string title)
		=> _dialogs.ShowConfirmAsync(message, title);
}