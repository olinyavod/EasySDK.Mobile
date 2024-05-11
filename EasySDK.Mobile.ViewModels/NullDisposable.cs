using System;

namespace EasySDK.Mobile.ViewModels;

public class NullDisposable : IDisposable
{
	public static IDisposable Instance { get; } = new NullDisposable();

	public void Dispose()
	{
			
	}
}