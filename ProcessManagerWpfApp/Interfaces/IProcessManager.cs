using System.Diagnostics;

namespace ProcessManagerWpfApp.Interfaces;

internal interface IProcessManager
{
    (int success, int fail) Kill(string name);
    void SetPriority(int processId, ProcessPriorityClass priority);
    void Start(string fileName);
}
