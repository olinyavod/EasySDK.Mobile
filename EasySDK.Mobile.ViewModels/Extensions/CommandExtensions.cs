using System.Windows.Input;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class CommandExtensions
{
    #region Public methods

    public static void TryExecute(this ICommand command, object parameter = null)
    {
        if(command?.CanExecute(parameter) != true)
            return;

        command.Execute(parameter);
    }

    #endregion
}