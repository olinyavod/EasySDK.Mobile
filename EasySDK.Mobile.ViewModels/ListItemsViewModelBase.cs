using System;
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
	
	private bool _isBusy;
	private string _searchQuery;
	private int _remainingItemsThreshold = -1;
	private int _itemsCount;

	#endregion

	#region Properties

	protected ILogger Log { get; }

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

	public int ItemsCount
	{
		get => _itemsCount;
		set => SetProperty(ref _itemsCount, value);
	}

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
		Log = logger ?? throw new ArgumentNullException(nameof(logger));

		LoadItemsCommand = new Command(OnLoadItems);
		LoadNextItemsCommand = new AsyncCommand(OnLoadNextItems);
	}

	#endregion

	#region Public methods

	public virtual void OnAppearing()
	{
		_searchQuery = string.Empty;
		OnPropertyChanged(nameof(SearchQuery));
		IsBusy = true;
	}

	#endregion

	#region Protected methods

	protected virtual Task<bool> OnPreLoadItems() => Task.FromResult(true);

	protected virtual Task OnPostLoadItems() => Task.FromResult(true);

	protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IListRequest request);

	protected abstract TItem CreateItem(TModel model);

	protected abstract string GetLoadItemsFailedMessage();

	protected int CalculateThreshold(IResponseList<TModel> response)
	{
		var threshold =  response.TotalCount - ItemsSource.Count;

		return threshold > 0 ? threshold : -1;
	}

	protected virtual string GetSearchQueryText()
	{
		return string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery;
	}

	protected async Task DoLoadItemsAsync(IListRequest request)
	{
		try
		{
			ItemsSource.Clear();

			if (!await OnPreLoadItems())
				return;

			var response = await LoadItemsAsync(request);

			IsBusy = !response.HasError;
			if (!await _responseChecker.CheckCanContinue(response, GetLoadItemsFailedMessage()))
				return;

			var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();

			foreach (var item in items)
				ItemsSource.Add(item);

			ItemsCount = response.TotalCount;
			RemainingItemsThreshold = CalculateThreshold(response);

			await OnPostLoadItems();
		}
		catch (Exception ex)
		{
			Log.LogError(ex, $"Load items error.");
			_dialogs.ShowErrorMessage(GetLoadItemsFailedMessage());
		}
		finally
		{
			IsBusy = false;
		}
	}

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
		await DoLoadItemsAsync(new ListRequest
		{
			Count = DefaultPageSize,
			Search = GetSearchQueryText()
		});
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
				Search = GetSearchQueryText()
			});

			if (!await _responseChecker.CheckCanContinue(response, GetLoadItemsFailedMessage()))
				return;

			var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();

			foreach (var item in items)
				ItemsSource.Add(item);

			ItemsCount = response.TotalCount;
			RemainingItemsThreshold = CalculateThreshold(response);
		}
		catch (Exception ex)
		{
			Log.LogError(ex, $"Load next items for error.");
			_dialogs.ShowErrorMessage(GetLoadItemsFailedMessage());
		}
	}

	#endregion
}