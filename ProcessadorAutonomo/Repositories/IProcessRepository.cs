using AutonomousTaskProcessor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutonomousTaskProcessor.Repositories
{
    public interface IProcessRepository : IRepository<Process>
    {
        void UpdateProcessStatus(int processId, StatusProcess newStatus);
        void ConcludeSubprocess(SubProcess subProcess);
    }
}
