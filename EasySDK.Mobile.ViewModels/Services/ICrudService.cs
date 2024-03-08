using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.ViewModels.Services;

public interface ICrudService<TModel, TItem, TKey>
{
	Task<IResponseList<TItem>> GetListAsync(IListRequest request);

	Task<IResponse<TModel>> GetByKeyAsync(TKey key);

	Task<IResponse<TKey>> AddAsync(TModel model);

	Task<IResponse<bool?>> UpdateAsync(Expression<Func<TModel>> model);

	Task<IResponse<bool?>> DeleteAsync(TKey id);
}