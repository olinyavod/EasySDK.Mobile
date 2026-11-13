using EasySDK.Mobile.Models;
using EasySDK.Mobile.RestClient.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace EasySDK.Mobile.ViewModels.Tests;

public class AsyncListResponseHookTests
{
	[Test]
	public async Task Rejected_initial_failure_is_not_successful_empty_and_busy_settles()
	{
		var list = new ListModel(new ScopeFactory());
		list.Response = new HttpResponseList<int> { ErrorCode = 500, ErrorMessage = "Unavailable" };
		await list.Reload();
		Assert.That(list.HasLoadError, Is.True);
		Assert.That(list.IsEmpty, Is.False);
		Assert.That(list.IsBusy, Is.False);
		Assert.That(list.ItemsSource, Is.Null);
	}

	[Test]
	public async Task Rejected_refresh_retains_exact_loaded_collection()
	{
		var list = new ListModel(new ScopeFactory())
			{ Response = new HttpResponseList<int>(new[] { 1, 2 }) };
		await list.Reload();
		var loaded = list.ItemsSource;
		list.Response = new HttpResponseList<int> { ErrorCode = 500, ErrorMessage = "Unavailable" };
		await list.Reload();
		Assert.That(list.ItemsSource, Is.SameAs(loaded));
		Assert.That(list.ItemsSource, Is.EqualTo(new[] { 1, 2 }));
		Assert.That(list.IsEmpty, Is.False);
		Assert.That(list.IsBusy, Is.False);
	}

	[Test]
	public async Task Accepted_empty_response_clears_error_and_reports_successful_empty()
	{
		var list = new ListModel(new ScopeFactory());
		list.Response = new HttpResponseList<int> { ErrorCode = 500, ErrorMessage = "Unavailable" };
		await list.Reload();
		list.Response = new HttpResponseList<int>(Array.Empty<int>());
		await list.Reload();
		Assert.That(list.IsEmpty, Is.True);
		Assert.That(list.HasLoadError, Is.False);
		Assert.That(list.IsBusy, Is.False);
	}

	[Test]
	public async Task Superseded_loading_finally_does_not_clear_newer_busy_state()
	{
		var list = new ListModel(new ScopeFactory());
		var first = list.Reload();
		var second = list.Reload();
		await first;
		Assert.That(list.IsBusy, Is.True, "The newer loader still owns the busy flag.");
		await second;
		Assert.That(list.IsBusy, Is.False);
	}

	[TestCase(false)]
	[TestCase(true)]
	public async Task Checker_rejection_keeps_legacy_empty_semantics_unless_explicitly_opted_in(bool optIn)
	{
		UnoptedList list = optIn
			? new OptedList(new ScopeFactory(), throwLoad: false, reject: true)
			: new UnoptedList(new ScopeFactory(), throwLoad: false, reject: true);
		await list.Reload();
		Assert.That(list.IsEmpty, Is.EqualTo(!optIn));
		Assert.That(list.IsBusy, Is.False);
		Assert.That(list.ItemsSource, Is.Null);
		Assert.That(list.ErrorPresentations, Is.Zero);
	}

	[TestCase(false)]
	[TestCase(true)]
	public async Task Load_exception_keeps_legacy_empty_semantics_unless_explicitly_opted_in(bool optIn)
	{
		UnoptedList list = optIn
			? new OptedList(new ScopeFactory(), throwLoad: true, reject: false)
			: new UnoptedList(new ScopeFactory(), throwLoad: true, reject: false);
		await list.Reload();
		Assert.That(list.IsEmpty, Is.EqualTo(!optIn));
		Assert.That(list.IsBusy, Is.False);
		Assert.That(list.ItemsSource, Is.Null);
		Assert.That(list.ErrorPresentations, Is.EqualTo(1));
	}

	// Deliberately does not override UseResponseErrorState: this represents an unchanged SDK consumer.
	private class UnoptedList(IServiceScopeFactory scopeFactory, bool throwLoad, bool reject)
		: AsyncListItemsViewModelBase<int, int>(scopeFactory,
			reject ? new RejectChecker() : new AcceptChecker(), NullLogger.Instance)
	{
		public int ErrorPresentations { get; private set; }
		public Task Reload() => LoadItemsAsync(true);
		protected override Task<IResponseList<int>> LoadItemsAsync(IServiceProvider scope, IListRequest request, CancellationToken token)
		{
			if (throwLoad) throw new IOException("Synthetic load failure.");
			return Task.FromResult<IResponseList<int>>(new HttpResponseList<int>(Array.Empty<int>()));
		}
		protected override int CreateItem(int model) => model;
		protected override string GetLoadItemsFailedMessage() => "Failed";
		protected override void ShowErrorMessage(string message) => ErrorPresentations++;
	}

	private sealed class OptedList(IServiceScopeFactory scopeFactory, bool throwLoad, bool reject)
		: UnoptedList(scopeFactory, throwLoad, reject)
	{
		protected override bool UseResponseErrorState => true;
	}

	private sealed class ListModel(IServiceScopeFactory scopeFactory)
		: AsyncListItemsViewModelBase<int, int>(scopeFactory, new AcceptChecker(), NullLogger.Instance)
	{
		protected override bool UseResponseErrorState => true;
		public IResponseList<int> Response { get; set; } = new HttpResponseList<int>(Array.Empty<int>());
		public Task Reload() => LoadItemsAsync(true);
		protected override Task<IResponseList<int>> LoadItemsAsync(IServiceProvider scope, IListRequest request, CancellationToken token)
			=> Task.FromResult(Response);
		protected override Task<IResponseList<int>?> ExecuteLoadItemsResponseAsync(IServiceProvider scope, IListRequest request, CancellationToken token)
		{
			SetLoadError(Response.HasError ? Response.ErrorMessage : null);
			return Task.FromResult(Response.HasError ? null : Response);
		}
		protected override int CreateItem(int model) => model;
		protected override string GetLoadItemsFailedMessage() => "Failed";
		protected override void ShowErrorMessage(string message) => Assert.Fail(message);
	}

	private sealed class AcceptChecker : IResponseChecker
	{
		public Task<bool> CheckCanContinue(IServiceProvider scope, IResponse response, string message) => Task.FromResult(true);
	}

	private sealed class RejectChecker : IResponseChecker
	{
		public Task<bool> CheckCanContinue(IServiceProvider scope, IResponse response, string message) => Task.FromResult(false);
	}

	private sealed class ScopeFactory : IServiceScopeFactory
	{
		public IServiceScope CreateScope() => new Scope();
	}

	private sealed class Scope : IServiceScope, IServiceProvider
	{
		public IServiceProvider ServiceProvider => this;
		public object? GetService(Type serviceType) => null;
		public void Dispose() { }
	}
}
