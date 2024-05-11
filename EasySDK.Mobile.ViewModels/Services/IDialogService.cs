using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IDialogService
{
	#region Methods

	Task<TResult?> ShowAsync<TViewModel, TResult>(Action<TViewModel>? configure = null)
		where TViewModel : class, IDialogViewModel;

	IShowScope<TViewModel> Show<TViewModel>(Action<TViewModel>? configure = null) 
		where TViewModel : class;

	#endregion
}