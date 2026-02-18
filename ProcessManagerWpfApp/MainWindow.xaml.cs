using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ProcessManagerWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadProcesses();
            priorityComboBox.SelectedIndex = 2;
        }

        private void LoadProcesses()
        {
            processesDataGrid.Items.Clear();
            List<ProcessInfo> processes = ProcessManager.GetProcesses();

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

            var (success, fail) = ProcessManager.KillProcesses(selected.Name);

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
                ProcessManager.SetPriority(selected.Id, priority);
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
            ProcessManager.StartProgram("calc.exe");
        }

        private void startWordButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessManager.StartProgram("WINWORD.exe");
        }

        private void startNotepadButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessManager.StartProgram("notepad.exe");
        }

        private void startBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessManager.StartProgram("https://www.google.com");
        }

        private void startExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessManager.StartProgram("explorer.exe");
        }
    }
}
