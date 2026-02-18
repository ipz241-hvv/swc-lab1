namespace ProcessManagerWpfApp.Models
{
    public class ProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double MemoryMb { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int Threads { get; set; }
    }
}
