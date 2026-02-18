using System.Diagnostics;
using ProcessManagerWpfApp.Interfaces;

namespace ProcessManagerWpfApp.Models;

internal class WindowsProcessProvider : IProcessProvider
{
    public List<ProcessInfo> GetProcessInfos()
    {
        Process[] processes = Process.GetProcesses();
        List<ProcessInfo> list = MapProcessesToProcessInfos(processes);

        return list;
    }

    private List<ProcessInfo> MapProcessesToProcessInfos(Process[] processes)
    {
        List<ProcessInfo> list = new List<ProcessInfo>();

        foreach (Process process in processes)
        {
            ProcessInfo info = new ProcessInfo();
            info.Id = process.Id;
            info.Name = process.ProcessName;

            try { info.MemoryMb = Math.Round(process.WorkingSet64 / (1024.0 * 1024.0), 2); }
            catch { info.MemoryMb = 0; }

            try { info.StartTime = process.StartTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            catch { info.StartTime = "н/д"; }

            try { info.Priority = process.PriorityClass.ToString(); }
            catch { info.Priority = "н/д"; }

            try { info.Threads = process.Threads.Count; }
            catch { info.Threads = 0; }

            list.Add(info);
        }

        return list;
    }
}
