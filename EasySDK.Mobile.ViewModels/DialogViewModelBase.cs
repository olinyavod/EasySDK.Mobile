using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.ViewModels;

public abstract class DialogViewModelBase : ViewModelBase, IDialogViewModel
{
	#region Private fields

	private IDialogServiceOwner? _owner;

	#endregion

	#region Properties
	
	IDialogServiceOwner? IDialogViewModel.Owner
	{
		get => _owner;
		set => _owner = value;
	}

	#endregion

	#region Protected methods

	protected void Close(object? result = null)
	{
		_owner?.Close(result);
	}

	#endregion
}