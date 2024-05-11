using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.ViewModels;

public interface IDialogViewModel
{
	public IDialogServiceOwner? Owner { get; set; }
}