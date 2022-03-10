using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Input;

public class AsyncCommand : IAsyncCommand
{
	#region Private fiedls

	private readonly WeakEventManager _weakEventManager = new();

	private readonly Func<Task> _onExecute;
	private readonly Func<bool> _onCanExecute;
	private bool _isBusy;

	#endregion

	#region Events

	public event EventHandler CanExecuteChanged
	{
		add => _weakEventManager.AddEventHandler(value);
		remove => _weakEventManager.RemoveEventHandler(value);
	}

	public event PropertyChangedEventHandler PropertyChanged;

	#endregion

	#region Properties

	public bool IsBusy
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value, ChangeCanExecute);
	}

	#endregion

	#region ctor

	protected AsyncCommand()
	{

	}

	public AsyncCommand(Func<Task> onExecute, Func<bool> onCanExecute = null)
	{
		_onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		_onCanExecute = onCanExecute;
	}

	#endregion

	#region Public methods

	public void ChangeCanExecute()
	{
		_weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
	}

	public bool CanExecute(object parameter) => !IsBusy && CanExecuteCore(parameter);

	public async void Execute(object parameter)
	{
		try
		{
			IsBusy = true;
			await ExecuteCore(parameter);
		}
		finally
		{
			IsBusy = false;
		}
	}

	#endregion

	#region Protected methods

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		onChanged?.Invoke();
		OnPropertyChanged(propertyName);
		return true;
	}

	protected virtual bool CanExecuteCore(object parameter) => _onCanExecute?.Invoke() ?? true;
	

	protected virtual Task ExecuteCore(object parameter) => _onExecute();

	#endregion
}

public class AsyncCommand<TParameter> : AsyncCommand
{
	#region Private fields

	private readonly Func<TParameter, Task> _onExecute;
	private readonly Func<TParameter, bool> _onCanExecute;

	#endregion

	#region ctor

	public AsyncCommand(Func<TParameter, Task> onExecute, Func<TParameter, bool> onCanExecute = null)
	{
		_onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
		_onCanExecute = onCanExecute;
	}

	#endregion

	#region Protected methods

	protected override bool CanExecuteCore(object parameter)
	{
		TParameter actualParameter = GetActualParameter(parameter);

		return _onCanExecute?.Invoke(actualParameter) ?? true;
	}

	protected override Task ExecuteCore(object parameter)
	{
		var acrualParameter = GetActualParameter(parameter);
		return _onExecute(acrualParameter);
	}

	#endregion

	#region Private methods

	private TParameter GetActualParameter(object parameter) => parameter switch
	{
		TParameter p => p,

		_ => default
	};

	#endregion
}