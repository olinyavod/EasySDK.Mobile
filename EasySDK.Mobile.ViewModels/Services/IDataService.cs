using System.Threading;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.ViewModels.Services;

public interface IDataService<TModel, TItem, TKey>
{
	Task<IResponseList<TItem>> GetListAsync(IListRequest request, CancellationToken token);

	Task<IResponse<TModel?>> GetByIdAsync(TKey id, CancellationToken cancellationToken);
}