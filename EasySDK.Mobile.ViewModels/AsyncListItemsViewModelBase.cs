using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using EasySDK.Mobile.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.ViewModels
{
	public abstract class AsyncListItemsViewModelBase<TItem, TModel> : ScopedViewModelBase
	{
		#region Private fields

		private readonly IResponseChecker _responseChecker;

		private ObservableCollection<TItem>?               _itemsSource;
		private bool                        _isBusy;
		private bool                        _isEmpty = true;
		private CancellationTokenSource?    _loadCancelSource;
		private TaskCompletionSource<bool>? _loadingTask;
		private string? _loadErrorMessage;
		private bool _loadRejected;
		private int _loadVersion;
		private bool _hasSuccessfulLoad;

		#endregion

		#region Properties

		public ObservableCollection<TItem>? ItemsSource
		{
			get => _itemsSource;
			set => SetProperty(ref _itemsSource, value);
		}

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		public int PageSize { get; set; } = 30;

		public string? LoadErrorMessage
		{
			get => _loadErrorMessage;
			private set
			{
				if (SetProperty(ref _loadErrorMessage, value)) OnPropertyChanged(nameof(HasLoadError));
			}
		}

		public bool HasLoadError => !string.IsNullOrWhiteSpace(LoadErrorMessage);

		protected ILogger Log { get; private set; }

		public bool IsEmpty
		{
			get => _isEmpty;
			set => SetProperty(ref _isEmpty, value);
		}

		#endregion

		protected AsyncListItemsViewModelBase
		(
			IServiceScopeFactory scopeFactory,
			IResponseChecker responseChecker,
			ILogger          logger
		) : base(scopeFactory)
		{
			_responseChecker = responseChecker ?? throw new ArgumentNullException(nameof(responseChecker));

			Log = logger;
			
			LoadNextItemsCommand = new RelayCommand(OnLoadNext);
			LoadItemsCommand     = new RelayCommand(OnLoadItems);
		}

		#region Protected methods

		/// <summary>Application opt-in; existing SDK consumers retain their original empty-state semantics.</summary>
		protected virtual bool UseResponseErrorState => false;

		protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IServiceProvider scope, IListRequest request,
			CancellationToken                                                          token);

		/// <summary>Optional application hook. Return null to reject a classified response before replacing data.</summary>
		protected virtual async Task<IResponseList<TModel>?> ExecuteLoadItemsResponseAsync(IServiceProvider scope,
			IListRequest request, CancellationToken token) => await LoadItemsAsync(scope, request, token);

		protected void SetLoadError(string? message) => LoadErrorMessage = message;

		protected async Task LoadItemsAsync(bool clearAllItems)
		{
			var version = Interlocked.Increment(ref _loadVersion);
			if (_loadCancelSource is { } activeLoad)
			{
				if (!activeLoad.IsCancellationRequested) activeLoad.Cancel();

				if (_loadingTask?.Task is { } loadingTask)
					await loadingTask;
				else
					await Task.Yield();
			}
			if (version != _loadVersion) return;

			CancellationTokenSource? cancelSource = null;
			IEnumerable<TItem>? newItems = null;
			TaskCompletionSource<bool>? loadingCompletion = null;

			try
			{
				IsBusy = true;
				IsEmpty = false;
				_loadRejected = false;
				SetLoadError(null);

				_loadingTask      = loadingCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
				_loadCancelSource = cancelSource = new CancellationTokenSource();
				await Task.Delay(250, cancelSource.Token);

				await using var scope = CreateAsyncScope();

				newItems = await DoLoadItemsAsync(scope.ServiceProvider, new ListRequest
				{
					Offset = clearAllItems ? 0 : ItemsSource?.Count ?? 0, Count = PageSize
				}, clearAllItems, cancelSource.Token);

				if (cancelSource.IsCancellationRequested)
					throw new OperationCanceledException();

			}
			catch (OperationCanceledException)
			{
				if (newItems == null)
					return;

				foreach (var item in newItems)
					ItemsSource?.Remove(item);
			}
			finally
			{
				if (version == _loadVersion)
				{
					IsBusy = false;
					IsEmpty = ItemsSource?.Any() is not true
						&& (!UseResponseErrorState || !HasLoadError && !_loadRejected && _hasSuccessfulLoad);
				}
				cancelSource?.Dispose();
				if (ReferenceEquals(_loadCancelSource, cancelSource)) _loadCancelSource = null;
				loadingCompletion?.TrySetResult(true);
			}
		}

		protected abstract void ShowErrorMessage(string message);

		protected virtual Task<bool> OnPreLoadItems(IServiceProvider scope) => Task.FromResult(true);

		protected virtual Task OnPostLoadItems(IServiceProvider scope) => Task.CompletedTask;

		protected abstract string GetLoadItemsFailedMessage();

		protected abstract TItem CreateItem(TModel model);

		#endregion

		#region Private methods

		private async Task<IEnumerable<TItem>> DoLoadItemsAsync(IServiceProvider scope,         IListRequest      request,
			bool                                                                 clearAllItems, CancellationToken token)
		{
			LinkedList<TItem> addedItems = new();
			var previousItems = ItemsSource;
			var replacedItems = false;
			try
			{
				if (!await OnPreLoadItems(scope))
					return addedItems;

				var response = await ExecuteLoadItemsResponseAsync(scope, request, token);

				if (token.IsCancellationRequested)
					throw new OperationCanceledException();

				if (response is null)
				{
					_loadRejected = true;
					return addedItems;
				}

				if (!await _responseChecker.CheckCanContinue(scope, response, GetLoadItemsFailedMessage()))
				{
					_loadRejected = true;
					return addedItems;
				}

				if (token.IsCancellationRequested)
					throw new OperationCanceledException();

				var items = response.Result?.Select(CreateItem).ToArray() ?? Array.Empty<TItem>();
				bool hasItems = false;

				if (ItemsSource is null || clearAllItems)
				{
					ItemsSource = new();
					replacedItems = true;
				}

				foreach (var item in items)
				{
					if (token.IsCancellationRequested)
						throw new OperationCanceledException();

					hasItems = true;
					ItemsSource?.Add(item);
					addedItems.AddLast(item);
				}

				if (!hasItems)
					await Task.Delay(500, token);

				if (token.IsCancellationRequested)
					throw new OperationCanceledException();

				await OnPostLoadItems(scope);
				_hasSuccessfulLoad = true;

				return addedItems;
			}
			catch (WebException ex)
			{
				if (string.Compare(ex.Message, "Canceled", StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					foreach (var item in addedItems)
						ItemsSource?.Remove(item);

					throw new OperationCanceledException();
				}

				Log.LogError(ex, $"Load items error.");
				ShowErrorMessage(GetLoadItemsFailedMessage());

				return addedItems;
			}
			catch (OperationCanceledException)
			{
				foreach (var item in addedItems)
					ItemsSource?.Remove(item);
				if (replacedItems) ItemsSource = previousItems;

				throw;
			}
			catch (Exception ex)
			{
				Log.LogError(ex, $"Load items error.");
				ShowErrorMessage(GetLoadItemsFailedMessage());

				return addedItems;
			}
		}

		#endregion

		#region LoadItemsCommand

		public ICommand LoadItemsCommand { get; }

		private void OnLoadItems()
		{
			_ = LoadItemsAsync(true);
		}
		
		#endregion //LoadItemsCommand

		#region LoadNextItemsCommand

		public ICommand LoadNextItemsCommand { get; }

		private void OnLoadNext()
		{
			_ = LoadItemsAsync(false);
		}
		
		#endregion //LoadNextItemsCommand
	}
}
