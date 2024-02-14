using AutonomousTaskProcessor.Entities;

namespace TaskProcessorAPI.Dtos
{
    public class ReadProcessDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public StatusProcess Status { get; set; } = StatusProcess.Created;

        public DateTime StartedAt { get; set; }

        public DateTime EndedAt { get; set; }

        public IList<SubProcess> SubProcesses { get; set; } = new List<SubProcess>();
    }
}
