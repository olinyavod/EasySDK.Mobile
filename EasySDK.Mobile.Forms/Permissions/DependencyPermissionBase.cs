using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Permissions;

public abstract class DependencyPermissionBase<TPermission> : Xamarin.Essentials.Permissions.BasePermission
	where TPermission : class, IPlatformPermission
{
	#region Private fields

	private readonly TPermission _platformPermission;

	#endregion

	#region ctor

	protected DependencyPermissionBase()
	{
		_platformPermission = DependencyService.Resolve<TPermission>();
	}

	#endregion

	#region Public methods

	public override Task<PermissionStatus> CheckStatusAsync() => _platformPermission?.CheckStatusAsync() ?? Task.FromResult(PermissionStatus.Unknown);

	public override Task<PermissionStatus> RequestAsync() => _platformPermission?.RequestAsync() ?? Task.FromResult(PermissionStatus.Unknown);

	public override void EnsureDeclared() => _platformPermission?.EnsureDeclared();

	public override bool ShouldShowRationale() => _platformPermission?.ShouldShowRationale() ?? false;

	#endregion
}