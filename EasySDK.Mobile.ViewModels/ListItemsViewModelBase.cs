using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.ViewModels;

public abstract class ListItemsViewModelBase<TItem, TModel> : ViewModelBase, ISupportAppearing
	where TItem : ViewModelBase
	where TModel : class
{
	#region Private fields

	private const int DefaultPageSize = 30;
	private readonly IResponseChecker _responseChecker;
	
	private bool _isBusy;
	private string? _searchQuery;
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

	public string? SearchQuery
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
		IServiceScopeFactory scopeFactory,
		IResponseChecker     responseChecker,
		ILogger              logger
	) : base(scopeFactory)
	{
		_responseChecker = responseChecker ?? throw new ArgumentNullException(nameof(responseChecker));
		Log              = logger ?? throw new ArgumentNullException(nameof(logger));

		LoadItemsCommand     = new RelayCommand(OnLoadItems);
		LoadNextItemsCommand = new AsyncRelayCommand(OnLoadNextItems, OnCanLoadNext);
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

	protected abstract void ShowErrorMessage(string message);

	protected virtual Task<bool> OnPreLoadItems(IServiceProvider scope) => Task.FromResult(true);

	protected virtual Task OnPostLoadItems(IServiceProvider scope) => Task.FromResult(true);

	protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IServiceProvider scope, IListRequest request);

	protected abstract TItem CreateItem(TModel model);

	protected abstract string GetLoadItemsFailedMessage();

	protected int CalculateThreshold(IResponseList<TModel> response)
	{
		var threshold =  response.TotalCount - ItemsSource.Count;

		return threshold > 0 ? threshold : -1;
	}

	protected virtual string? GetSearchQueryText()
	{
		return string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery;
	}

	protected async Task DoLoadItemsAsync(IServiceProvider scope, IListRequest request)
	{
		try
		{
			ItemsSource.Clear();

			if (!await OnPreLoadItems(scope))
				return;

			var response = await LoadItemsAsync(scope, request);

			IsBusy = !response.HasError;
			if (!await _responseChecker.CheckCanContinue(scope, response, GetLoadItemsFailedMessage()))
				return;

			var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();

			foreach (var item in items)
				ItemsSource.Add(item);

			ItemsCount = response.TotalCount;
			RemainingItemsThreshold = CalculateThreshold(response);

			await OnPostLoadItems(scope);
		}
		catch (Exception ex)
		{
			Log.LogError(ex, $"Load items error.");
			ShowErrorMessage(GetLoadItemsFailedMessage());
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

	protected virtual bool OnCanLoad() => !IsBusy;

	private async void OnLoadItems()
	{
		await using var scope = CreateAsyncScope();
		await DoLoadItemsAsync(scope.ServiceProvider, new ListRequest
		{
			Count = DefaultPageSize,
			Search = GetSearchQueryText()
		});
	}

	#endregion

	#region LoadNextItemsCommand

	public ICommand LoadNextItemsCommand { get; }

	protected virtual bool OnCanLoadNext() => !IsBusy;

	private async Task OnLoadNextItems()
	{
		try
		{
			await using var scope = CreateAsyncScope();
			var offset = ItemsSource.Count;
			var response = await LoadItemsAsync(scope.ServiceProvider, new ListRequest
			{
				Count  = DefaultPageSize,
				Offset = offset,
				Search = GetSearchQueryText()
			});

			if (!await _responseChecker.CheckCanContinue(scope.ServiceProvider, response, GetLoadItemsFailedMessage()))
				return;

			var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();

			foreach (var item in items)
				ItemsSource.Add(item);

			ItemsCount              = response.TotalCount;
			RemainingItemsThreshold = CalculateThreshold(response);
		}
		catch (Exception ex)
		{
			Log.LogError(ex, $"Load next items for error.");
			ShowErrorMessage(GetLoadItemsFailedMessage());
		}
		finally
		{
			IsBusy = false;
		}
	}

	#endregion
}	