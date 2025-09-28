using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IPageOwner
{
	void CLose(object? result = null);
}

public interface IPageViewModel
{
	IPageOwner? Owner { get; set; }
}

public class ShellNavigationService : INavigationService, IPageOwner
{
	private readonly Dictionary<Page, TaskCompletionSource<object?>> _pagesResult = new();

	private bool _navigatingSubsribed;

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
	
	private void CurrentOnNavigating(object sender, ShellNavigatingEventArgs e)
	{
		var current = Shell.Current;
		var currentPage = current.CurrentPage;
		if (e.Source is ShellNavigationSource.Pop or ShellNavigationSource.PopToRoot
		    && _pagesResult.Remove(currentPage, out var task))
		{
			task.SetResult(null);
		}
	}

	public async Task<TResult?> GoToAsync<TResult>(string route)
	{
		var current = Shell.Current;

		if (!_navigatingSubsribed)
		{
			current.Navigating   += CurrentOnNavigating;
			_navigatingSubsribed =  true;
		}

		await current.GoToAsync(route);

		var pageViewModel = current.CurrentPage as IPageViewModel ?? CurrentPage.BindingContext as IPageViewModel;
		if (pageViewModel == null)
			return default;

		pageViewModel.Owner = this;

		var pageTask = new TaskCompletionSource<object?>();
		_pagesResult[current.CurrentPage] = pageTask;

		var result = await pageTask.Task;

		if (result != null)
			return (TResult) result;

		return default;
	}

	public void CLose(object? result = null)
	{
		var current = Shell.Current;
		if (!_pagesResult.Remove(current.CurrentPage, out var task))
		{
			current.GoToAsync("..");
			return;
		}

		task.SetResult(result);
		current.GoToAsync("..");
	}
}