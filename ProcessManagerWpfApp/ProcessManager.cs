using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessManagerWpfApp
{
    public static class ProcessManager
    {
        public static List<ProcessInfo> GetProcesses()
        {
            List<ProcessInfo> list = new List<ProcessInfo>();
            Process[] processes = Process.GetProcesses();

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

        public static (int success, int fail) KillProcesses(string name)
        {
            int success = 0;
            int fail = 0;

            Process[] processes = Process.GetProcessesByName(name);

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

        public static void SetPriority(int processId, ProcessPriorityClass priority)
        {
            Process process = Process.GetProcessById(processId);
            process.PriorityClass = priority;
        }

        public static void StartProgram(string fileName)
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
                throw new Exception($"Не вдалося запустити {fileName}: {ex.Message}", ex);
            }
        }
    }
}
