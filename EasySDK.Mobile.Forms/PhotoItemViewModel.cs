using EasySDK.Mobile.ViewModels;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms;

public class PhotoItemViewModel : NotifyPropertyChangedBase
{
	#region Properties

	public ImageSource? ImageSource { get; set; }

	public string? Id { get; set; }

	#endregion
}