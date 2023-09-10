using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public class ShellNavigationService : INavigationService
{
	public TViewModel? GetCurrentViewModel<TViewModel>() where TViewModel : class => Shell.Current?.CurrentPage?.BindingContext as TViewModel;

	public Task GoToAsync(ShellNavigationState state, bool animate = true) => Shell.Current.GoToAsync(state, animate);

	public Task GoToAsync(string baseUri, Dictionary<string, string?>? args = null, bool animate = true)
	{
		if (args != null)
			baseUri = $"{baseUri}?{string.Join("&", args.Where(i => !string.IsNullOrWhiteSpace(i.Value)).Select(i => $"{i.Key}={i.Value.ToUrlArgs()}"))}";

		return Shell.Current.GoToAsync(baseUri);
	}

	public Page CurrentPage => Shell.Current.CurrentPage;

	public IReadOnlyList<Page?> OpenPages => Shell.Current.Navigation.NavigationStack;

	public void RemovePage(Page page) => Shell.Current.Navigation.RemovePage(page);
}