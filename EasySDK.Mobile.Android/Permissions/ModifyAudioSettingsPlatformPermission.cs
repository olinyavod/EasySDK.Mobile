using EasySDK.Mobile.Android.Permissions;
using EasySDK.Mobile.Forms.Permissions;
using Xamarin.Forms;

[assembly:Dependency(typeof(ModifyAudioSettingsPlatformPermission))]
namespace EasySDK.Mobile.Android.Permissions;

public class ModifyAudioSettingsPlatformPermission : Xamarin.Essentials.Permissions.BasePlatformPermission, IModifyAudioSettingsPermission
{
	public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new[]
	{
		(global::Android.Manifest.Permission.ModifyAudioSettings, true)
	};
}