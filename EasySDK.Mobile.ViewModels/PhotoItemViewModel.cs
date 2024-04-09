using System;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public class PhotoItemViewModel : ViewModelBase
{
	#region Properties

	public ImageSource? ImageSource { get; set; }

	public string? Id { get; set; }

	#endregion
}