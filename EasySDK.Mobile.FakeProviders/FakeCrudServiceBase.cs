using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EasySDK.Mobile.FakeProviders.Models;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.FakeProviders;

public abstract class FakeCrudServiceBase<TModel, TKey>
{
	#region Private fields

	private readonly Func<TModel, TKey> _getKey;
	private readonly int _delayMilliseconds;

	#endregion

	#region Properties

	protected IFixture Fixture { get; }
	protected Dictionary<TKey, TModel> Items { get; }

	#endregion

	#region ctor

	protected FakeCrudServiceBase(int count, Func<TModel, TKey> getKey, int delayMilliseconds = 500)
	{
		_getKey = getKey;
		_delayMilliseconds = delayMilliseconds;
		Fixture = new Fixture();

		Items = GenerateItems(Fixture, count).ToDictionary(i => getKey(i), i => i);
	}

	#endregion

	#region Public methods

	public Task<IResponse<IEnumerable<TModel>>> GetList(IListRequest request) => Task.Run(async () =>
	{
		await Delay();
		return FakeResponse.FromResult(Items
			.Values
			.Where(i => Search(i, request.Search))
			.Skip(request.Offset)
			.Take(request.Count));
	});

	public Task<IResponse<TModel>> GetByKey(TKey key) => Task.Run(async () =>
	{
		await Delay();

		return Items.TryGetValue(key, out var value)
			? FakeResponse.FromResult(value)
			: FakeResponse.FromErrorCode<TModel>(ResponseErrorCodes.NotFound);
	});

	public Task<IResponse<bool>> Edit(TModel model) => Task.Run(async () =>
	{
		await Delay();
		var key = _getKey(model);

		if (!Items.ContainsKey(key))
			return FakeResponse.FromErrorCode<bool>(ResponseErrorCodes.NotFound);

		Items[key] = model;
		return FakeResponse.FromResult(true);
	});

	public Task<IResponse<bool>> Delete(TKey key) => Task.Run(async () =>
	{
		await Delay();

		return Items.Remove(key)
			? FakeResponse.FromResult(true)
			: FakeResponse.FromErrorCode<bool>(ResponseErrorCodes.NotFound);
	});

	#endregion

	#region Protected methods

	protected abstract bool Search(TModel model, string query);

	protected async Task Delay()
	{
		if(_delayMilliseconds == 0)
			return;

		await Task.Delay(_delayMilliseconds);
	}

	protected IEnumerable<TModel> GenerateItems(IFixture fixture, int count)
	{
		Configure(fixture);

		return fixture.CreateMany<TModel>(count);
	}

	protected abstract void Configure(IFixture fixture);

	#endregion
}

