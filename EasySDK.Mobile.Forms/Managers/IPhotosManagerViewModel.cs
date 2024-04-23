using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace EasySDK.Mobile.ViewModels.Managers;

public interface IPhotosManagerViewModel
{
	#region Events

	event PropertyChangedEventHandler PropertyChanged;

	#endregion

	#region Properties

	bool AllowDelete { get; }

	bool AllowAdd { get; }

	ObservableCollection<PhotoItemViewModel> PhotosSource { get; }

	bool IsPhotoOpened { get; set; }

	PhotoItemViewModel? SelectedPhoto { get; set; }

	int PhotosCount { get; set; }

	int SelectedPhotoIndex { get; set; }

	#endregion

	#region Commands

	ICommand AddPhotoCommand { get; }

	ICommand DeletePhotoCommand { get; }

	ICommand OpenPhotoCommand { get; }

	ICommand ClosePhotoCommand { get; }

	#endregion
}