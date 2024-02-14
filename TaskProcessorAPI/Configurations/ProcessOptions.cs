namespace TaskProcessorAPI.Configurations
{
    public class ProcessOptions
    {
        public const string ProcessesConfig = "ProcessesConfig";
        public int MaxParallelProcesses { get; set; }
        public int MinSubProcessesPerProcess { get; set; }
        public int MaxSubProcessesPerProcess { get; set; }
        public int MinSubProcessesDuration { get; set; }
        public int MaxSubProcessesDuration { get; set; }

    }
}
