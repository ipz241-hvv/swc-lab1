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
    public List<ProgramButtonInfo> ProgramButtonInfos { get; } = new()
    {
        new("Калькулятор", "calc.exe"),
        new("Word", "WINWORD.exe"),
        new("Блокнот", "notepad.exe"),
        new("Браузер", "https://www.google.com"),
        new("Провідник", "explorer.exe")
    };
    public string[] PriorityList { get; } = Enum.GetNames(typeof(ProcessPriorityClass));

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
    public ICommand StartCommand { get; }

    public MainViewModel(IProcessProvider provider, IProcessManager manager, IDialogService dialogService)
    {
        _processProvider = provider;
        _processManager = manager;
        _dialogService = dialogService;

        RefreshCommand = new RelayCommand(_ => LoadProcesses());
        KillCommand = new RelayCommand(_ => { KillSelected(); LoadProcesses(); }, _ => SelectedProcess != null);
        SetPriorityCommand = new RelayCommand(_ => { SetPriority(); LoadProcesses(); }, _ => SelectedProcess != null);
        StartCommand = new RelayCommand(path =>
        {
            if (path is string processPath)
            {
                _processManager.Start(processPath);
                LoadProcesses();
            }
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
    }
}