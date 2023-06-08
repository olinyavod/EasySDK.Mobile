using System;
using System.Threading;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient;

namespace EasySDK.Mobile.ViewModels.Services;

public class DeviceAccountTokenProvider : ITokenProvider
{
	#region Private fields

	private readonly IDeviceAccountService _deviceAccountService;
	private readonly object _sync = new();

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
		var locked = false;
		try
		{
			Monitor.TryEnter(_sync, TimeSpan.FromSeconds(10), ref locked);
			_token = null;

			return await _deviceAccountService.InvalidAuthToken();
		}
		finally
		{
			if (locked)
				Monitor.Exit(_sync);
		}
	}

	public async Task<string?> TryGetAuthTokenAsync()
	{
		var locked = false;
		try
		{
			var token = _token;
			if(!string.IsNullOrWhiteSpace(token))
				return token;

			Monitor.TryEnter(_sync, TimeSpan.FromSeconds(10), ref locked);
			
			token = await _deviceAccountService.TryGetAuthTokenAsync();
			_token = token;

			return token;
		}
		finally
		{
			if(locked)
				Monitor.Exit(_sync);
		}
	}

	#endregion
}