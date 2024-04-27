using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public enum DialogIcons
{
	None, Success, Error, Warning, Question, Information
}

public interface IDialogService
{
	Task AlertAsync(string message, string? title = null, string? ok = null, DialogIcons icon = DialogIcons.None);

	Task<bool> ConfirmAsync(string              message,    string? title = null, string? ok = null, string? cancel = null, DialogIcons icon = DialogIcons.None);

	IDisposable Loading(CancellationTokenSource cancelSource, string title);
}