﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySDK.Mobile.ViewModels.Pages;
using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.ViewModels;

public abstract class LogsViewModelBase : ScopedViewModelBase, ISupportAppearing
{
	#region Private fields

	private readonly IShareService              _shareService;
	private readonly IClipboardService          _clipboardService;
	private readonly IPathsService              _pathsService;
	private readonly ILogger<LogsViewModelBase> _log;

	private string? _text;
	private string? _title = Properties.Resources.Journal;

	#endregion

	#region Properties

	public string? Text
	{
		get => _text;
		set => SetProperty(ref _text, value);
	}

	public string? Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}

	#endregion

	#region ctor

	protected LogsViewModelBase
	(
		IShareService        shareService,
		IClipboardService    clipboardService,
		IServiceScopeFactory scopeFactory,
		IPathsService        pathsService,
		ILoggerFactory       loggerFactory
	) : base(scopeFactory)
	{
		if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
		_shareService     = shareService ?? throw new ArgumentNullException(nameof(shareService));
		_clipboardService = clipboardService;
		_pathsService     = pathsService ?? throw new ArgumentNullException(nameof(pathsService));

		_log = loggerFactory.CreateLogger<LogsViewModelBase>();

		ClearCommand   = new AsyncRelayCommand(OnClear);
		CopyAllCommand = new RelayCommand(OnCopyAll);
		ShareCommand   = new AsyncRelayCommand(OnShare);
	}

	#endregion

	#region Public methods

	public async void OnAppearing()
	{
		var logsFilePath = _pathsService.GetLogsFilePath();
		
		if(!File.Exists(logsFilePath))
			return;
		
		using var reader = new StreamReader(logsFilePath);
		var builder = new StringBuilder();

		for (int i = 0; i < 100 && !reader.EndOfStream; i++)
			builder.AppendLine(await reader.ReadLineAsync());

		Text = builder.ToString();
	}

	#endregion

	#region Protected methods

	protected abstract void ShowSuccessMessage(string message);

	protected abstract void ShowErrorMessage(string message);

	protected abstract Task<bool> ShowConfirmAsync(string message, string title);

	#endregion

	#region ClearCommand

	public ICommand ClearCommand { get; }

	private async Task OnClear()
	{
		try
		{
			if (!await ShowConfirmAsync
			    (
					Properties.Resources.ClearAllLogsMessage,
					Title!
			    ))
				return;

			Text = string.Empty;
			var logFilePath = _pathsService.GetLogsFilePath();
			var dir = new DirectoryInfo(Path.GetDirectoryName(logFilePath) ?? string.Empty);

			await Task.Run(() =>
			{
				if (!dir.Exists)
					return;

				var files = dir.GetFiles("*.log");

				foreach (var file in files.Where(i => i.Exists))
					file.Delete();
			});
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Clear logs error.");
			ShowErrorMessage(Properties.Resources.FailedClearLogsMessage);
		}
	}

	#endregion

	#region CopyAllCommand

	public ICommand CopyAllCommand { get; }

	private void OnCopyAll()
	{
		try
		{
			_clipboardService.SetTextAsync(Text ?? string.Empty);

			ShowSuccessMessage(Properties.Resources.CopyAllLogsMessage);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Copy logs error.");
			ShowErrorMessage(Properties.Resources.FailedCopyLogsMessage);
		}
	}

	#endregion

	#region ShareCommand

	public ICommand ShareCommand { get; }

	private async Task OnShare()
	{
		try
		{
			var file = _pathsService.GetLogsFilePath();

			if(!File.Exists(file))
				return;

			await _shareService.ShareFileAsync(file, "text/plain");
			//  Share.RequestAsync(new ShareFileRequest(Path.GetFileName(file), new ShareFile(file, "text/plain")));
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Share logs error.");
			ShowErrorMessage(Properties.Resources.FailedShareLogsMessage);
		}
	}

	#endregion
}