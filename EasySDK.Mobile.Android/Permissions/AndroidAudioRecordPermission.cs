#nullable enable

using Android;
using EasySDK.Mobile.Android.Permissions;
using EasySDK.Mobile.ViewModels.Permissions;
using Xamarin.Forms;

[assembly:Dependency(typeof(AndroidAudioRecordPermission))]
namespace EasySDK.Mobile.Android.Permissions
{
	public class AndroidAudioRecordPermission : Xamarin.Essentials.Permissions.BasePlatformPermission, IAudioRecordPermission
	{
		public override (string androidPermission, bool isRuntime)[] RequiredPermissions { get; } =
		{
			(Manifest.Permission.RecordAudio, true),
			//(Manifest.Permission.ReadContacts, true),
			//(Manifest.Permission.CaptureAudioOutput, true),
		};
	}
}
