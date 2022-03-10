using System.ComponentModel;
using System.Windows.Input;

namespace EasySDK.Mobile.ViewModels.Input;

public interface IAsyncCommand : ICommand, INotifyPropertyChanged
{
	#region Properties

	bool IsBusy { get; set; }

	#endregion

	#region Methods

	void ChangeCanExecute();

	#endregion
}