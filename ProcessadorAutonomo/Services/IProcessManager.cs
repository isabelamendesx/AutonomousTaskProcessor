using AutonomousTaskProcessor.Entities;

namespace AutonomousTaskProcessor.Services;

public interface IProcessManager
{
    Task Create();
    Task<Process?> Check(int processId);
    Task Cancel(int processId);
    Task<IEnumerable<Process>> ListActiveProcesses();
    Task<IEnumerable<Process>> ListInactiveProcesses();
}
