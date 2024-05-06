using System;
using System.Collections.Generic;
using System.Linq;
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

		private IList<TItem>?            _itemsSource;
		private bool                     _isBusy;
		private bool                     _isEmpty = true;
		private CancellationTokenSource? _loadCancelSource;

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

		protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IServiceProvider scope, IListRequest request,
			CancellationToken                                                          token);

		protected async Task DoLoadItemsAsync(IServiceProvider scope, IListRequest request, CancellationToken token)
		{
			LinkedList<TItem> addedItems = new();
			try
			{
				if (!await OnPreLoadItems(scope))
					return;

				var response = await LoadItemsAsync(scope, request, token);

				if (token.IsCancellationRequested)
					throw new OperationCanceledException();

				if (!await _responseChecker.CheckCanContinue(scope, response, GetLoadItemsFailedMessage()))
					return;

				if(token.IsCancellationRequested)
					throw new OperationCanceledException();
				
				var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();
				bool hasItems = false;

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
			}
		}

		protected abstract void ShowErrorMessage(string message);

		protected virtual Task<bool> OnPreLoadItems(IServiceProvider scope) => Task.FromResult(true);

		protected virtual Task OnPostLoadItems(IServiceProvider scope) => Task.CompletedTask;

		protected abstract string GetLoadItemsFailedMessage();

		protected abstract TItem CreateItem(TModel model);

		protected async Task LoadItemsAsync(bool clearAllItems)
		{
			if (_loadCancelSource?.IsCancellationRequested is false)
			{
				_loadCancelSource.Cancel();
				await Task.Yield();
			}

			CancellationTokenSource? cancelSource = null;

			try
			{
				IsBusy            = true;
				
				_loadCancelSource = cancelSource = new CancellationTokenSource();
				await Task.Delay(250, cancelSource.Token);

				if (ItemsSource is null || clearAllItems)
				{
					ItemsSource = new List<TItem>();
					await Task.Yield();
				}

				await using var scope = CreateAsyncScope();

				await DoLoadItemsAsync(scope.ServiceProvider, new ListRequest
				{
					Offset = ItemsSource.Count, Count = PageSize
				}, cancelSource.Token);

				IsEmpty = ItemsSource?.Any() is not true;
				IsBusy  = false;
			}
			catch (OperationCanceledException)
			{

			}
			finally
			{
				cancelSource?.Dispose();
				_loadCancelSource = null;


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