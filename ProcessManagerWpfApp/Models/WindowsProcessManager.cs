using System.Diagnostics;
using ProcessManagerWpfApp.Interfaces;

namespace ProcessManagerWpfApp.Models;

public class WindowsProcessManager : IProcessManager
{
    public WindowsProcessManager() { }

    public (int success, int fail) Kill(string name)
    {
        Process[] processes = Process.GetProcessesByName(name);

        (int success, int fail) = ProcessKill(processes);

        return (success, fail);
    }

    public void SetPriority(int processId, ProcessPriorityClass priority)
    {
        Process process = Process.GetProcessById(processId);
        
        ProcessSetPriority(process, priority);
    }

    public void Start(string fileName)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot launch {fileName}: {ex.Message}", ex);
        }
    }

    private (int success, int fail) ProcessKill(Process[] processes)
    {
        int success = 0;
        int fail = 0;

        foreach (Process process in processes)
        {
            try
            {
                process.Kill();
                success++;
            }
            catch
            {
                fail++;
            }
        }

        return (success, fail);
    }

    private void ProcessSetPriority(Process process, ProcessPriorityClass priority)
    {
        process.PriorityClass = priority;
    }
}
