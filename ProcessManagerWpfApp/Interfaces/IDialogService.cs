namespace ProcessManagerWpfApp.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title, bool isError = false);
    }
}
