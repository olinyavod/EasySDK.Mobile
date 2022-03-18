using EasySDK.Mobile.Android.Permissions;
using EasySDK.Mobile.ViewModels.Permissions;
using Xamarin.Forms;

[assembly:Dependency(typeof(AnswerRejectCallsPlatformPermission))]
namespace EasySDK.Mobile.Android.Permissions;

public class AnswerRejectCallsPlatformPermission : Xamarin.Essentials.Permissions.BasePlatformPermission, IAnswerRejectCallsPermission
{
	public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new[]
	{
		(global::Android.Manifest.Permission.AnswerPhoneCalls, true),
		(global::Android.Manifest.Permission.ReadPhoneState, true),
	};
}