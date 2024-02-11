using ProcessadorAutonomo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        void Add();
        void Update(T entity);

        void UpdateProcessStatus(int processId, StatusProcess newStatus);

        void ConcludeSubprocess(SubProcess subProcess);
    }
}
