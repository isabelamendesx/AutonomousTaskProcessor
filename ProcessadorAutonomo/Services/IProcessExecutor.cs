using ProcessadorAutonomo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Services
{
    public interface IProcessExecutor
    {
        Task ExecuteProcessAsync(Entities.Process process, CancellationToken cancellationToken);
        Task ExecuteSubProcessAsync(SubProcess subprocess, CancellationToken cancellationToken);
        Task ExecuteProcessInParallel();
        Task CancelProcess(int processId);
        Task Close();
    }
}
