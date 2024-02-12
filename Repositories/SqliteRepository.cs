using AutonomousTaskProcessor.Data;
using AutonomousTaskProcessor.Entities;
using AutonomousTaskProcessor.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories;

public class SqliteRepository : IProcessRepository
{
    private readonly ProcessContext _context;
    private readonly IConfiguration _config;
    private readonly object lockObject = new object();

    public SqliteRepository(ProcessContext context, IConfiguration configuration)
    {
        _context = context;
        _config = configuration;
    }

    public void Add()
    {
        var process = new Process
        {
            Name = "Process",
            SubProcesses = GenerateSubProcesses()
        };

        _context.Processes.Add(process);
        _context.SaveChanges();
    }

    public IEnumerable<Process> GetAll()
    {
        lock (lockObject)
        {
            return _context.Processes.Include(p => p.SubProcesses).ToList();
        }
    }

    public IEnumerable<SubProcess> GetAllSubprocesses()
    {
        lock (lockObject)
        {
            return _context.SubProcesses.ToList();
        }
    }

    public Process? GetById(int id)
    {
        lock (lockObject)
        {
            return _context.Processes.Include(p => p.SubProcesses).FirstOrDefault(process => process.Id == id);
        }
    }

    public void Update(Process updatedProcess)
    {
        var processToUpdate = _context.Processes.FirstOrDefault(process => process.Id == updatedProcess.Id);

        if (processToUpdate != null)
        {
            processToUpdate.Name = updatedProcess.Name;
            _context.SaveChanges();
        }
    }

    public void UpdateProcessStatus(int processId, StatusProcess statusProcess)
    {
        lock (lockObject)
        {
            var processToUpdate = GetById(processId);
            if (processToUpdate != null)
            {
                processToUpdate.Status = statusProcess;
                _context.SaveChangesAsync();
            }
        }
    }

    public void ConcludeSubprocess(SubProcess subProcess, bool concluded = true)
    {
        lock (lockObject)
        {
            subProcess.isConcluded = concluded;
            _context.SaveChanges();
        }
    }

    private List<SubProcess> GenerateSubProcesses()
    {
        var random = new Random();
        var subProcesses = new List<SubProcess>();
        var subProcessQuantity = random.Next(int.Parse(_config.GetSection("MinSubProcessesPerProcess").Value ?? "1"), int.Parse(_config.GetSection("MaxSubProcessesPerProcess").Value ?? "2"));

        for (int i = 0; i < subProcessQuantity; i++)
        {
            subProcesses.Add(
                 new SubProcess
                 {
                     Duration = TimeSpan.FromSeconds(random.Next(int.Parse(_config.GetSection("MinSubProcessesDuration").Value ?? "1"), int.Parse(_config.GetSection("MaxSubProcessesDuration").Value ?? "3")))
                 }
            );
        }

        return subProcesses;
    }

}
