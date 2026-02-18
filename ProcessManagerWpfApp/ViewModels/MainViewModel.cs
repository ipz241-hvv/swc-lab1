using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ProcessManagerWpfApp.Interfaces;
using ProcessManagerWpfApp.Models;

namespace ProcessManagerWpfApp.ViewModels;

internal class MainViewModel : BaseViewModel
{
    private readonly IProcessProvider _processProvider;
    private readonly IProcessManager _processManager;
    private readonly IDialogService _dialogService;

    public ObservableCollection<ProcessInfo> Processes { get; } = new();

    private ProcessInfo? _selectedProcess;
    public ProcessInfo? SelectedProcess
    {
        get => _selectedProcess;
        set { _selectedProcess = value; OnPropertyChanged(); }
    }

    private string _selectedPriority = "Normal";
    public string SelectedPriority
    {
        get => _selectedPriority;
        set { _selectedPriority = value; OnPropertyChanged(); }
    }

    public ICommand RefreshCommand { get; }
    public ICommand KillCommand { get; }
    public ICommand SetPriorityCommand { get; }
    public ICommand StartCalcCommand { get; }
    public ICommand StartWordCommand { get; }
    public ICommand StartNotepadCommand { get; }
    public ICommand StartBrowserCommand { get; }
    public ICommand StartExplorerCommand { get; }

    public MainViewModel(IProcessProvider provider, IProcessManager manager, IDialogService dialogService)
    {
        _processProvider = provider;
        _processManager = manager;
        _dialogService = dialogService;
        _dialogService = dialogService;

        RefreshCommand = new RelayCommand(_ => LoadProcesses());
        KillCommand = new RelayCommand(_ => KillSelected(), _ => SelectedProcess != null);
        SetPriorityCommand = new RelayCommand(_ => SetPriority(), _ => SelectedProcess != null);
        StartCalcCommand = new RelayCommand(_ => {
            _processManager.Start("calc.exe");
            LoadProcesses();
        });
        StartWordCommand = new RelayCommand(_ => {
            _processManager.Start("WINWORD.exe");
            LoadProcesses();
        });
        StartNotepadCommand = new RelayCommand(_ => {
            _processManager.Start("notepad.exe");
            LoadProcesses();
        });
        StartBrowserCommand = new RelayCommand(_ => {
            _processManager.Start("https://www.google.com");
            LoadProcesses();
        });
        StartExplorerCommand = new RelayCommand(_ => {
            _processManager.Start("explorer.exe");
            LoadProcesses();
        });

        LoadProcesses();
    }

    private void LoadProcesses()
    {
        var processes = _processProvider.GetProcessInfos();
        
        Processes.Clear();
        
        foreach (var process in processes)
        {
            Processes.Add(process);
        }
    }

    private void KillSelected()
    {
        if (SelectedProcess == null)
        {
            _dialogService.ShowMessage("Виберіть процес у таблиці.", "Увага");
            return;
        }

        var (success, fail) = _processManager.Kill(SelectedProcess.Name);

        LoadProcesses();

        _dialogService.ShowMessage($"Успішно завершено: {success}\nНе вдалося завершити: {fail}", "Результат");
    }

    private void SetPriority()
    {
        if (SelectedProcess == null)
        {
            _dialogService.ShowMessage("Виберіть процес у таблиці.", "Увага");
            return;
        }

        if (!Enum.TryParse(SelectedPriority, out ProcessPriorityClass priority))
            priority = ProcessPriorityClass.Normal;

        try
        {
            _processManager.SetPriority(SelectedProcess.Id, priority);
            _dialogService.ShowMessage($"Пріоритет процесу {SelectedProcess.Name} змінено на {priority}.", "Готово");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Не вийшло змінити пріоритет:\n{ex.Message}", "Помилка", true);
        }

        LoadProcesses();
    }
}