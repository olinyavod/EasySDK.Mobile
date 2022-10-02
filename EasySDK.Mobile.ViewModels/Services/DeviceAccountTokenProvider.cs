using System;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.ViewModels.Services;

public class DeviceAccountTokenProvider : ITokenProvider
{
	#region Private fields

	private readonly IDeviceAccountService _deviceAccountService;

	#endregion
	
	#region ctor

	public DeviceAccountTokenProvider(IDeviceAccountService deviceAccountService)
	{
		_deviceAccountService = deviceAccountService;
	}

	#endregion

	#region Public methods

	public bool InvalidateToken() => _deviceAccountService.InvalidAuthToken();

	public Task<string?> TryGetAuthTokenAsync() => _deviceAccountService.TryGetAuthTokenAsync();

	#endregion
}