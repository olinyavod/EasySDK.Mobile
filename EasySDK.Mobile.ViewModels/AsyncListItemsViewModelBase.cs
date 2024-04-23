using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EasySDK.Mobile.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels
{
	public abstract class AsyncListItemsViewModelBase<TItem, TModel> : ViewModelBase
	{
		#region Private fields

		private readonly IResponseChecker _responseChecker;

		private IList<TItem>? _itemsSource;
		private bool          _isBusy;
		private bool          _isEmpty = true;

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

			LoadNextItemsCommand = new Command(OnLoadNext);
			LoadItemsCommand     = new Command(OnLoadItems);
		}

		#region Protected methods

		protected abstract Task<IResponseList<TModel>> LoadItemsAsync(IServiceProvider scope, IListRequest request);

		protected async Task DoLoadItemsAsync(IServiceProvider scope, IListRequest request)
		{
			try
			{
				if (!await OnPreLoadItems(scope))
					return;

				var response = await LoadItemsAsync(scope, request);

				IsBusy = !response.HasError;
				if (!await _responseChecker.CheckCanContinue(scope, response, GetLoadItemsFailedMessage()))
					return;

				
				var items = response.Result?.Select(CreateItem) ?? Enumerable.Empty<TItem>();
				bool hasItems = false;

				foreach (var item in items)
				{
					hasItems = true;
					ItemsSource?.Add(item);
				}

				if (!hasItems)
					await Task.Delay(1000);

				await OnPostLoadItems(scope);
			}
			catch (Exception ex)
			{
				Log.LogError(ex, $"Load items error.");
				MainThread.BeginInvokeOnMainThread(() => ShowErrorMessage(GetLoadItemsFailedMessage()));
			}
		}

		protected abstract void ShowErrorMessage(string message);

		protected virtual Task<bool> OnPreLoadItems(IServiceProvider scope) => Task.FromResult(true);

		protected virtual Task OnPostLoadItems(IServiceProvider scope) => Task.CompletedTask;

		protected abstract string GetLoadItemsFailedMessage();

		protected abstract TItem CreateItem(TModel model);

		#endregion

		#region Private methods

		private async Task LoadItemsAsync(bool clearAllItems)
		{
			await Task.Run(async () =>
			{
				if(ItemsSource is null || clearAllItems)
					ItemsSource = new List<TItem>();

				await using var scope = CreateAsyncScope();

				await DoLoadItemsAsync(scope.ServiceProvider, new ListRequest
				{
					Offset = ItemsSource.Count, Count = PageSize
				});
			});
			IsEmpty = ItemsSource?.Any() is not true;
			IsBusy  = false;
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