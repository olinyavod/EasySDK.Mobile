#nullable enable

using Android;
using Android.OS;
using EasySDK.Mobile.Android.Permissions;
using EasySDK.Mobile.ViewModels.Permissions;
using Xamarin.Forms;

[assembly:Dependency(typeof(AndroidAudioRecordPermission))]

namespace EasySDK.Mobile.Android.Permissions
{
	public class AndroidAudioRecordPermission : Xamarin.Essentials.Permissions.BasePlatformPermission,
		IAudioRecordPermission
	{
		public override (string androidPermission, bool isRuntime)[] RequiredPermissions
		{
			get
			{
				if (Build.VERSION.SdkInt > BuildVersionCodes.S)
					return new[]
					{
						(Manifest.Permission.RecordAudio, true),
						(Manifest.Permission.BluetoothConnect, true),
						("android.permission.NEARBY_WIFI_DEVICES", true)
					};

				return new[] {(Manifest.Permission.RecordAudio, true)};

			}
		}
	}
}
