using System.Windows;
using ProcessManagerWpfApp.Models;
using ProcessManagerWpfApp.ViewModels;

namespace ProcessManagerWpfApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var processProvider = new WindowsProcessProvider();
        var processManager = new WindowsProcessManager();
        var dialogService = new WindowsDialogService();

        DataContext = new MainViewModel(processProvider, processManager, dialogService);
    }
}