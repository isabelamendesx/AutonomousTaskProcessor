namespace AutonomousTaskProcessor.Entities;

public interface IProcess
{
    int Id { get; }
    StatusProcess Status { get; }
    DateTime StartedAt { get; }
    DateTime EndedAt { get; }
    ICollection<SubProcess>? SubProcesses { get; }
}

public class Process : IProcess
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public StatusProcess Status { get; set; } = StatusProcess.Created;

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public ICollection<SubProcess> SubProcesses { get; set; } = new List<SubProcess>();
    public override string? ToString()
    {
        return $"Process {Id,-5} | Status: {Status,-10} | Subprocesses: {SubProcesses.Count}";
    }
}
