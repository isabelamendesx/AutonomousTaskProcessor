using Microsoft.Extensions.Configuration;
using ProcessadorAutonomo.Entities;
using ProcessadorAutonomo.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Services
{
    public class ProcessExecutor : IProcessExecutor
    {
        private IRepository<Entities.Process> _repository;
        public List<Task> RunningTasks;
        private Dictionary<int, CancellationTokenSource> _cancellationTokenSources;
        private SemaphoreSlim _semaphore;
        private readonly IConfiguration _configuration;
        private bool _running = true;

        public ProcessExecutor(IRepository<Entities.Process> repository, IConfiguration configuration)
        {
            _repository = repository;
            RunningTasks = new List<Task>();
            _cancellationTokenSources = new Dictionary<int, CancellationTokenSource>();
            _configuration = configuration;
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

            foreach (var cancellationTokenSource in _cancellationTokenSources)
            {
                var processId = cancellationTokenSource.Key;
                cancellationTokenSource.Value.Cancel();
                Console.WriteLine($"Process {processId} paused requested.");
               _repository.UpdateProcessStatus(processId, StatusProcess.Paused);
            }

            return Task.CompletedTask;
        }

        public async Task ExecuteProcessInParallel()
        {
            int maxParallelProcesses = int.Parse(_configuration.GetSection("MaxParallelProcesses").Value ?? "2");
            _semaphore = new SemaphoreSlim(maxParallelProcesses);

            _repository.GetAll().ToList().ForEach(process => _repository.UpdateProcessStatus(process.Id, StatusProcess.Scheduled));


            foreach (var process in _repository.GetAll())
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

        public async Task ExecuteSubProcessAsync(SubProcess subprocess, CancellationToken cancellationToken)
        {
                await Task.Delay(subprocess.Duration, cancellationToken);
                _repository.ConcludeSubprocess(subprocess);           
        }

        public async Task ExecuteProcessAsync(Entities.Process process, CancellationToken cancellationToken)
        {
            _repository.UpdateProcessStatus(process.Id, StatusProcess.InProgress);

            var pendingSubProcesss = process.SubProcesses.ToList();

            foreach(var subProcess in  pendingSubProcesss)
            {
                try
                {
                    await ExecuteSubProcessAsync(subProcess, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Subprocess {subProcess.Id} was canceled.");
                    return;
                }
            }

            _repository.UpdateProcessStatus(process.Id, StatusProcess.Completed);
            Console.WriteLine($"process {process.Id} completed\n"); 
        }
    }
}
