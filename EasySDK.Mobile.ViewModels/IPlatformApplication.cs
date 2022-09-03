using System;

namespace EasySDK.Mobile.ViewModels;

public interface IPlatformApplication
{
	static IPlatformApplication? Current { get; set; }

	IServiceProvider? ServiceProvider { get; }
}