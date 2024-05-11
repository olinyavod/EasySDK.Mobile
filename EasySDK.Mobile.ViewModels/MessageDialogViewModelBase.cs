using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.ViewModels;

public abstract class MessageDialogViewModelBase : DialogViewModelBase
{
	#region Private fields

	private string?     _title;
	private string?     _message;
	private DialogIcons _icon;

	#endregion

	#region Properties

	public string? Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}

	public string? Message
	{
		get => _message;
		set => SetProperty(ref _message, value);
	}

	public DialogIcons Icon
	{
		get => _icon;
		set => SetProperty(ref _icon, value);
	}

	#endregion
}