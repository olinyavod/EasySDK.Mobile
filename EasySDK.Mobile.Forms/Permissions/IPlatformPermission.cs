using System.Threading.Tasks;
using Xamarin.Essentials;

namespace EasySDK.Mobile.Forms.Permissions;

public interface IPlatformPermission
{
	#region Methods

	Task<PermissionStatus> CheckStatusAsync();

	Task<PermissionStatus> RequestAsync();

	void EnsureDeclared();

	bool ShouldShowRationale();

	#endregion
}