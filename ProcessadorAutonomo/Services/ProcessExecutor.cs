using AutonomousTaskProcessor.Entities;
using AutonomousTaskProcessor.Repositories;
using Microsoft.Extensions.Configuration;

namespace AutonomousTaskProcessor.Services;

public class ProcessExecutor : IProcessExecutor
{
    private IProcessRepository _repository;
    public List<Task> RunningTasks;
    private Dictionary<int, CancellationTokenSource> _cancellationTokenSources;
    private SemaphoreSlim? _semaphore;
    private readonly IConfiguration _config;
    private bool _running = true;

    public ProcessExecutor(IProcessRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        RunningTasks = new List<Task>();
        _cancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
        _config = configuration;
    }

    public Task CancelProcess(int processId)
    {
        if (_cancellationTokenSources.TryGetValue(processId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            _repository.UpdateProcessStatus(processId, StatusProcess.Canceled);
            Console.WriteLine($"Process {processId} cancellation requested.");
        }
        else
        {
            Console.WriteLine($"Process {processId} not found or already completed.");
        }

        return Task.CompletedTask;
    }

    public Task Close()
    {
        _running = false;

        foreach (var kvp in _cancellationTokenSources)
        { 
            var processId = kvp.Key;
            kvp.Value.Cancel();
            Console.WriteLine($"Process {processId} paused requested.");
            _repository.UpdateProcessStatus(processId, StatusProcess.Paused);
        }

        return Task.CompletedTask;
    }

    private IEnumerable<Process> generateProcessQueue()
    {
        var validStatus = new[] { StatusProcess.Created, StatusProcess.Scheduled, StatusProcess.Paused };

        var generateProcessQueue = _repository.GetAll()
            .Where(p => validStatus.Contains(p.Status))
            .ToList();

        generateProcessQueue.ForEach(process => _repository.UpdateProcessStatus(process.Id, StatusProcess.Scheduled));

        return generateProcessQueue;
    }

    public Task Restart()
    {
        _repository.GetAll().ToList().ForEach(process => _repository.UpdateProcessStatus(process.Id, StatusProcess.Scheduled));
        _repository.GetAllSubprocesses().ToList().ForEach(subprocess => _repository.ConcludeSubprocess(subprocess, false));

        return Task.CompletedTask;
    }

    public async Task Start()
    { 
        int maxParallelProcesses = int.Parse(_config.GetSection("ProcessesConfig")["MaxParallelProcesses"] ?? "5");
        _semaphore = new SemaphoreSlim(maxParallelProcesses);
        _running = true;

        foreach (var process in generateProcessQueue())
        {
            await _semaphore.WaitAsync();

            if (!_running)
            {
                _semaphore.Release();
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSources.Add(process.Id, cancellationTokenSource);

            RunningTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await ExecuteProcessAsync(process, cancellationTokenSource.Token);
                }
                finally
                {
                    _semaphore.Release();
                    _cancellationTokenSources.Remove(process.Id);
                }
            }));
        }

        await Task.WhenAll(RunningTasks);
    }

    private async Task ExecuteSubProcessAsync(SubProcess subprocess, CancellationToken cancellationToken)
    {
        await Task.Delay(subprocess.Duration, cancellationToken);
        _repository.ConcludeSubprocess(subprocess);
    }

    private async Task ExecuteProcessAsync(Process process, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Process {process.Id} STARTED");
        _repository.UpdateProcessStatus(process.Id, StatusProcess.InProgress);

        var pendingSubProcesss = process.SubProcesses.ToList();

        foreach (var subProcess in pendingSubProcesss)
        {
            try
            {
                await ExecuteSubProcessAsync(subProcess, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        _repository.UpdateProcessStatus(process.Id, StatusProcess.Completed);
        Console.WriteLine($"\nProcess {process.Id} COMPLETED");
    }
}
