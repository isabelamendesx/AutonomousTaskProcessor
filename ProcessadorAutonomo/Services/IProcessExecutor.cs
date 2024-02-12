
namespace AutonomousTaskProcessor.Services;

public interface IProcessExecutor
{
    Task Start();
    Task CancelProcess(int processId);
    Task Close();
    Task Restart();
}
