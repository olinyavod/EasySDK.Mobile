namespace EasySDK.Mobile.Maui.Behaviors;

public class ShellBehavior : Behavior<Shell>
{
	protected override void OnAttachedTo(Shell shell)
	{
		base.OnAttachedTo(shell);

		shell.Navigated += ShellOnNavigated;
		shell.Navigating += ShellOnNavigating;
	}

	protected override void OnDetachingFrom(Shell shell)
	{
		base.OnDetachingFrom(shell);

		shell.Navigated  -= ShellOnNavigated;
		shell.Navigating -= ShellOnNavigating;
	}

	private void ShellOnNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		var shell = (Shell) sender!;
		var currentPage = shell.CurrentPage;

		if(currentPage is INavigatedListener pageListener)
			pageListener.OnNavigated();

		if (currentPage is {BindingContext: INavigatedListener listener})
			listener.OnNavigated();
	}

	private async void ShellOnNavigating(object? sender, ShellNavigatingEventArgs e)
	{
		if (!e.CanCancel || e.Cancelled)
			return;

		var shell = (Shell) sender!;

		if (shell.CurrentPage is not {BindingContext: INavigatingListener listener})
			return;

		var deferral = e.GetDeferral();

		if (!await listener.OnNavigating())
			e.Cancel();

		deferral.Complete();
	}
}