using Foundation;
using UIKit;

namespace EasySDK.Mobile.iOS;

[Preserve(AllMembers = true)]
public static class AppDelegateExtensions
{
	/// <summary>Check runtime environment</summary>
	/// <returns>
	/// <c>true</c> - when application run in debug mode or install from <i>Test Flight</i><br/>
	/// <c>false</c> - when application run in release mode or install from <i>App Store</i>
	/// </returns>
	public static bool IsDevelopmentEnvironment(this NSObject _)
	{
#if DEBUG || HOTRELOAD
		return true;
#endif

		const string testFlight = "sandboxReceipt";

		var mainBundle = NSBundle.MainBundle;
		return mainBundle.AppStoreReceiptUrl.LastPathComponent == testFlight;
	}
}