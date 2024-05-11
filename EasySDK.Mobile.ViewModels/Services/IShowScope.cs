using System;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IShowScope<out TViewModel> : IDisposable, IAsyncDisposable
	where TViewModel : class
{
	TViewModel? DataContext { get; }
}