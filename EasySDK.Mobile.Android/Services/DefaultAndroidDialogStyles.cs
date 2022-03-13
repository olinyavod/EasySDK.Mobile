using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Android.Services;

public class DefaultAndroidDialogStyles : IAndroidDialogStyles
{
	#region Properties

	public int AlertLightStyleId => Resource.Style.AlertLightDialog;

	public int AlertDarkStyleId => Resource.Style.AlertDarkDialog;

	#endregion
}