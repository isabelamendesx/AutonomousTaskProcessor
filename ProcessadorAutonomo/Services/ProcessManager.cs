using AutonomousTaskProcessor.Entities;
using AutonomousTaskProcessor.Repositories;
using Microsoft.Extensions.Configuration;

namespace AutonomousTaskProcessor.Services;

public class ProcessManager : IProcessManager
{
    private readonly IProcessRepository _repository;

    public ProcessManager(IProcessRepository repository)
    {
        _repository = repository;
    }

    public Task Create()
    {
        _repository.Add();
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Process>> ListActiveProcesses()
    {
        var activeStatus = new[] { StatusProcess.Created, StatusProcess.Scheduled, StatusProcess.InProgress, StatusProcess.Paused };
        return await Task.FromResult(
            _repository.GetAll()
            .Where(process => activeStatus.Contains(process.Status))
            );
    }

    public async Task<IEnumerable<Process>> ListInactiveProcesses()
    {
        var inactiveStatus = new[] { StatusProcess.Completed, StatusProcess.Canceled };
        return await Task.FromResult(
            _repository.GetAll()
            .Where(process => inactiveStatus.Contains(process.Status))
            );
    }

    public async Task<Process?> Check(int processId) =>
       await Task.FromResult(_repository.GetById(processId));

    public Task Cancel(int processId)
    {
        _repository.UpdateProcessStatus(processId, StatusProcess.Canceled);
        return Task.CompletedTask;
    }

}
