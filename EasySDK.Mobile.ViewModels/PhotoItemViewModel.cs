using System;
using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels;

public class PhotoItemViewModel : ViewModelBase
{
	#region Properties

	public ImageSource? ImageSource { get; set; }

	public string? Id { get; set; }

	/// <summary>
	/// Путь к локальному файлу фото (для фото, ещё не загруженных на сервер)
	/// </summary>
	public string? LocalFilePath { get; set; }

	/// <summary>
	/// Признак того, что фото ещё не загружено на сервер
	/// </summary>
	public bool IsPendingUpload => !string.IsNullOrEmpty(LocalFilePath) && string.IsNullOrEmpty(Id);

	#endregion
}