using System.Windows;
using ProcessManagerWpfApp.Interfaces;

namespace ProcessManagerWpfApp.Views;

internal class WindowsDialogService : IDialogService
{
    public void ShowMessage(string message, string title, bool isError = false)
    {
        var image = isError ? MessageBoxImage.Error : MessageBoxImage.Information;
        MessageBox.Show(message, title, MessageBoxButton.OK, image);
    }
}
