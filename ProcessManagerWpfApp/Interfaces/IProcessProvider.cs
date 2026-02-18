namespace ProcessManagerWpfApp.Interfaces;

internal interface IProcessProvider
{
    List<ProcessInfo> GetProcessInfos();
}
