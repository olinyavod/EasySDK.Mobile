using System;
using System.Threading;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.ViewModels.Services;

public class DeviceAccountTokenProvider : ITokenProvider
{
	#region Private fields

	private readonly IDeviceAccountService _deviceAccountService;
	private readonly SemaphoreSlim _semaphore = new(0);
	private readonly ILogger _logger;

	private string? _token;

	#endregion
	
	#region ctor

	public DeviceAccountTokenProvider
	(
		IDeviceAccountService deviceAccountService,
		ILoggerFactory loggerFactory
	)
	{
		_deviceAccountService = deviceAccountService;
		_logger = loggerFactory.CreateLogger<DeviceAccountTokenProvider>();
	}

	#endregion

	#region Public methods

	public virtual async Task<bool> InvalidateToken()
	{
		try
		{
			if(!await _semaphore.WaitAsync(TimeSpan.FromSeconds(10)))
				_logger.LogInformation("Detect deadlock on invalidate token.");
			_token = null;

			return await _deviceAccountService.InvalidAuthToken();
		}
		finally
		{
			_semaphore.Release();
		}
	}

	public virtual async Task<string?> TryGetAuthTokenAsync()
	{
		try
		{
			var token = _token;
			if(!string.IsNullOrWhiteSpace(token))
				return token;

			if(!await _semaphore.WaitAsync(TimeSpan.FromSeconds(10)))
				_logger.LogInformation("Detect deadlock on GetAurhToken.");
			
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