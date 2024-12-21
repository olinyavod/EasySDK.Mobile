namespace EasySDK.Mobile.Maui.Services;

public class ShellViewModelNavigationService : ShellViewModelNavigationServiceBase
{
	#region Private methods

	protected override Shell GetShell() => Shell.Current;

	

	#endregion
}