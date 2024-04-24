using System;

namespace EasySDK.Mobile.Forms;

public interface IPlatformApplication
{
	static IPlatformApplication? Current { get; set; }

	IServiceProvider? ServiceProvider { get; }
}