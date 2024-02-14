
using System.Text.Json.Serialization;

namespace AutonomousTaskProcessor.Entities;

public class SubProcess
{
    public int Id { get; set; }

    public TimeSpan Duration { get; set; }

    public bool isConcluded { get; set; } = false;

    [JsonIgnore]
    public int ProcessId { get; set; }

    [JsonIgnore]
    public Process Process { get; set; } = null!;

}
