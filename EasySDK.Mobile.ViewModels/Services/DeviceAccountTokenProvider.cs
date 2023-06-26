using System;
using System.Threading;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.ViewModels.Services;

public class DeviceAccountTokenProvider : ITokenProvider
{
	#region Private fields

	private readonly IDeviceAccountService _deviceAccountService;
	private readonly SemaphoreSlim _semaphore = new(0);

	private string? _token;

	#endregion
	
	#region ctor

	public DeviceAccountTokenProvider(IDeviceAccountService deviceAccountService)
	{
		_deviceAccountService = deviceAccountService;
	}

	#endregion

	#region Public methods

	public async Task<bool> InvalidateToken()
	{
		try
		{
			await _semaphore.WaitAsync(TimeSpan.FromSeconds(10));
			_token = null;

			return await _deviceAccountService.InvalidAuthToken();
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public async Task<string?> TryGetAuthTokenAsync()
	{
		try
		{
			var token = _token;
			if(!string.IsNullOrWhiteSpace(token))
				return token;

			await _semaphore.WaitAsync(TimeSpan.FromSeconds(10));
			
			token = await _deviceAccountService.TryGetAuthTokenAsync();
			_token = token;

			return token;
		}
		finally
		{
			_semaphore.Release();
		}
	}

	#endregion
}