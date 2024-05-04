namespace EasySDK.Mobile.Maui;

public interface INavigatingListener
{
	Task<bool> OnNavigating();
}