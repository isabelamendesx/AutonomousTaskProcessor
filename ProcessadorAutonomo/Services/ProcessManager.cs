using Microsoft.Extensions.Configuration;
using ProcessadorAutonomo.Entities;
using ProcessadorAutonomo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Services
{
    public class ProcessManager : IProcessManager
    {
        private readonly IRepository<Process> _repository;
        private readonly IConfiguration _configuration;

        public ProcessManager(IRepository<Process> repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public Task Create()
        {
            _repository.Add();
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Process>> ListActiveProcesses()
        {
            var activeStatus = new[] { StatusProcess.Created, StatusProcess.Scheduled , StatusProcess.InProgress, StatusProcess.Paused };
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
}
