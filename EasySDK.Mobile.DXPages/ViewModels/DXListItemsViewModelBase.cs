using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels;
using EasySDK.Mobile.ViewModels.Extensions;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace EasySDK.Mobile.DXPages.ViewModels
{
	public abstract class DXListItemsViewModelBase<TItem, TModel> : ViewModelBase
	{
		#region Private fields

		private readonly IUserDialogs     _dialogs;
		private readonly IResponseChecker _responseChecker;

		private IList<TItem>?               _itemsSource;
		private bool                        _isBusy;
		private bool                        _isEmpty = true;
		private CancellationTokenSource?    _loadCancelSource;
		private TaskCompletionSource<bool>? _loadingTask;

		#endregion

		#region Properties

		public IList<TItem>? ItemsSource
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

		protected ILogger Log { get; private set; }

		public bool IsEmpty
		{
			get => _isEmpty;
			set => SetProperty(ref _isEmpty, value);
		}

		#endregion

		protected DXListItemsViewModelBase
		(
			IUserDialogs     dialogs,
			IResponseChecker responseChecker,
			ILogger          logger
		)
		{
			_dialogs         = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
			_responseChecker = responseChecker ?? throw new ArgumentNullException(nameof(responseChecker));

			Log = logger;

			LoadNextItemsCommand = new Command(OnLoadNext);
			LoadItemsCommand     = new Command(OnLoadItems);
		}

		#region Protected methods

		protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IServiceProvider scope, IListRequest request,
			CancellationToken                                                          token);

		protected async Task<IEnumerable<TItem>> DoLoadItemsAsync(IServiceProvider scope, IListRequest request,
			bool clearAllItems, CancellationToken token)
		{
			LinkedList<TItem> addedItems = new();
			try
			{
				if (!await OnPreLoadItems(scope))
					return addedItems;

				var response = await LoadItemsAsync(scope, request, token);

				if (token.IsCancellationRequested)
					throw new OperationCanceledException();

				if (!await _responseChecker.CheckCanContinue(scope, response, GetLoadItemsFailedMessage()))
					return addedItems;

				if(token.IsCancellationRequested)
					throw new OperationCanceledException();
				
				var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();
				bool hasItems = false;

				if (ItemsSource is null || clearAllItems)
					ItemsSource = new List<TItem>();

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

				return addedItems;
			}
			catch (OperationCanceledException)
			{
				foreach (var item in addedItems)
					ItemsSource?.Remove(item);

				throw;
			}
			catch (Exception ex)
			{
				Log.LogError(ex, $"Load items error.");
				ShowErrorMessage(GetLoadItemsFailedMessage());

				return addedItems;
			}
		}

		private void ShowErrorMessage(string message)
		{
			_dialogs.ShowErrorMessage(message);
		}

		protected async Task LoadItemsAsync(bool clearAllItems)
		{
			if (_loadCancelSource?.IsCancellationRequested is false)
			{
				_loadCancelSource.Cancel();

				if (_loadingTask?.Task is { } loadingTask)
					await loadingTask;
				else
					await Task.Yield();
			}

			CancellationTokenSource? cancelSource = null;
			IEnumerable<TItem>? newItems = null;

			try
			{
				IsBusy = true;

				_loadingTask      = new TaskCompletionSource<bool>();
				_loadCancelSource = cancelSource = new CancellationTokenSource();
				await Task.Delay(250, cancelSource.Token);

				await using var scope = CreateAsyncScope();

				newItems = await DoLoadItemsAsync(scope.ServiceProvider, new ListRequest
				{
					Offset = clearAllItems ? 0 : ItemsSource?.Count ?? 0, Count = PageSize
				}, clearAllItems, cancelSource.Token);

				if (cancelSource.IsCancellationRequested)
					throw new OperationCanceledException();

				IsEmpty = ItemsSource?.Any() is not true;
				IsBusy  = false;
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
				cancelSource?.Dispose();
				_loadCancelSource = null;
				_loadingTask?.TrySetResult(true);
			}
		}

		protected virtual Task<bool> OnPreLoadItems(IServiceProvider scope) => Task.FromResult(true);

		protected virtual Task OnPostLoadItems(IServiceProvider scope) => Task.CompletedTask;

		protected abstract string GetLoadItemsFailedMessage();

		protected abstract TItem CreateItem(TModel model);

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