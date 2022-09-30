using System;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient;
using Xamarin.Essentials;

namespace EasySDK.Mobile.ViewModels.Services;

public class DeviceAccountTokenProvider : ITokenProvider
{
	#region Private fields

	private readonly IDeviceAccountService _deviceAccountService;

	#endregion

	#region Properties

	public string? Token
	{
		get => Preferences.Get(nameof(Token), string.Empty);
		set => Preferences.Set(nameof(Token), value);
	}

	#endregion

	#region ctor

	public DeviceAccountTokenProvider(IDeviceAccountService deviceAccountService)
	{
		_deviceAccountService = deviceAccountService;
	}

	#endregion

	#region Public methods

	public Task<string?> TruGetTokenAsync() => _deviceAccountService.TryGetAccountTokenAsync();

	#endregion
}