using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Extensions;
using EasySDK.Mobile.ViewModels.Input;
using EasySDK.Mobile.ViewModels.Pages;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public abstract class ListItemsViewModelBase<TItem, TModel> : ViewModelBase, ISupportAppearing
	where TItem : ViewModelBase
	where TModel : class
{
	#region Private fields

	private const int DefaultPageSize = 30;

	private readonly IUserDialogs _dialogs;
	private readonly IResponseChecker _responseChecker;
	private readonly ILogger _logger;

	private bool _isBusy;
	private string _searchQuery;
	private int _remainingItemsThreshold = -1;

	#endregion

	#region Properties

	public bool IsBusy
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value);
	}

	public string SearchQuery
	{
		get => _searchQuery;
		set => SetProperty(ref _searchQuery, value, SearchQueryOnChanged);
	}

	public int RemainingItemsThreshold
	{
		get => _remainingItemsThreshold;
		set => SetProperty(ref _remainingItemsThreshold, value);
	}

	public ObservableCollection<TItem> ItemsSource { get; } = new();

	#endregion

	#region ctor

	protected ListItemsViewModelBase
	(
		IUserDialogs dialogs,
		IResponseChecker responseChecker,
		ILogger logger
	)
	{
		_dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
		_responseChecker = responseChecker ?? throw new ArgumentNullException(nameof(responseChecker));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));

		LoadItemsCommand = new Command(OnLoadItems);
		LoadNextItemsCommand = new AsyncCommand(OnLoadNextItems);
	}

	#endregion

	#region Public methods

	public void OnAppearing()
	{
		_searchQuery = string.Empty;
		OnPropertyChanged(nameof(SearchQuery));
		IsBusy = true;
	}

	#endregion

	#region Protected methods

	protected virtual Task<bool> OnPreLoadItems() => Task.FromResult(true);

	protected virtual Task OnPostLoadItems() => Task.FromResult(true);

	protected abstract Task<IResponse<IEnumerable<TModel>>> LoadItemsAsync(IListRequest request);

	protected abstract TItem CreateItem(TModel model);

	protected abstract string GetLoadItemsFailedMessage();

	#endregion

	#region Private methods

	private void SearchQueryOnChanged()
	{
		IsBusy = true;
	}

	#endregion

	#region LoadItemsCommand

	public ICommand LoadItemsCommand { get; }

	private async void OnLoadItems()
	{
		try
		{
			ItemsSource.Clear();

			if (!await OnPreLoadItems())
				return;

			var getResponse = await LoadItemsAsync(new ListRequest
			{
				Count = DefaultPageSize, 
				Search = SearchQuery
			});

			IsBusy = !getResponse.HasError;
			if (!await _responseChecker.CheckCanContinue(getResponse, GetLoadItemsFailedMessage()))
				return;

			var items = getResponse.Result.Select(CreateItem);

			foreach (var item in items)
				ItemsSource.Add(item);

			RemainingItemsThreshold = 0;

			await OnPostLoadItems();

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Load items error.");
			_dialogs.ShowErrorMessage(GetLoadItemsFailedMessage());
		}
		finally
		{
			IsBusy = false;
		}
	}

	#endregion

	#region LoadNextItemsCommand

	public ICommand LoadNextItemsCommand { get; }

	private async Task OnLoadNextItems()
	{
		try
		{
			var offset = ItemsSource.Count;
			var response = await LoadItemsAsync(new ListRequest
			{
				Count = DefaultPageSize,
				Offset = offset, 
				Search = SearchQuery
			});

			if (!await _responseChecker.CheckCanContinue(response, GetLoadItemsFailedMessage()))
				return;

			var items = response.Result.Select(CreateItem);

			foreach (var item in items)
				ItemsSource.Add(item);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Load next items for error.");
			_dialogs.ShowErrorMessage(GetLoadItemsFailedMessage());
		}
	}

	#endregion
}