using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ProcessManagerWpfApp.Interfaces;
using ProcessManagerWpfApp.Models;

namespace ProcessManagerWpfApp;

public partial class MainWindow : Window
{
    IProcessManager _processManager;
    
    public MainWindow()
    {
        InitializeComponent();
        LoadProcesses();
        _processManager = new WindowsProcessManager();
        priorityComboBox.SelectedIndex = 2;
    }

    private void LoadProcesses()
    {
        IProcessProvider processProvider = new WindowsProcessProvider();
        processesDataGrid.Items.Clear();
        List<ProcessInfo> processes = processProvider.GetProcessInfos();

        foreach (ProcessInfo info in processes)
        {
            processesDataGrid.Items.Add(info);
        }
    }

    private ProcessInfo? GetSelectedProcess()
    {
        return processesDataGrid.SelectedItem as ProcessInfo;
    }

    private void refreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadProcesses();
    }

    private void killProcessButton_Click(object sender, RoutedEventArgs e)
    {
        ProcessInfo? selected = GetSelectedProcess();
        
        if (selected == null)
        {
            MessageBox.Show("Виберіть процес у таблиці.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var (success, fail) = _processManager.Kill(selected.Name);

        LoadProcesses();

        MessageBox.Show($"Успішно завершено: {success}\nНе вдалося завершити: {fail}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void setPriorityButton_Click(object sender, RoutedEventArgs e)
    {
        ProcessInfo? selected = GetSelectedProcess();
        
        if (selected == null)
        {
            MessageBox.Show("Виберіть процес у таблиці.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (priorityComboBox.SelectedItem is not ComboBoxItem item)
        {
            MessageBox.Show("Оберіть пріоритет у списку.", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string priorityName = item.Content.ToString() ?? "Normal";
        
        if (!Enum.TryParse(priorityName, out ProcessPriorityClass priority))
            priority = ProcessPriorityClass.Normal;

        try
        {
            _processManager.SetPriority(selected.Id, priority);
            MessageBox.Show($"Пріоритет процесу {selected.Name} змінено на {priority}.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не вдалося змінити пріоритет:\n{ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        LoadProcesses();
    }

    private void startCalcButton_Click(object sender, RoutedEventArgs e)
    {
        _processManager.Start("calc.exe");
    }

    private void startWordButton_Click(object sender, RoutedEventArgs e)
    {
        _processManager.Start("WINWORD.exe");
    }

    private void startNotepadButton_Click(object sender, RoutedEventArgs e)
    {
        _processManager.Start("notepad.exe");
    }

    private void startBrowserButton_Click(object sender, RoutedEventArgs e)
    {
        _processManager.Start("https://www.google.com");
    }

    private void startExplorerButton_Click(object sender, RoutedEventArgs e)
    {
        _processManager.Start("explorer.exe");
    }
}
