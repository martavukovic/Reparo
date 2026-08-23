using System.Net.Mail;

namespace Reparo.Shared.Models;

public class Intervention
{
    public int Id { get; set; }

    public int WorkAssignmentId { get; set; }
    public WorkAssignment WorkAssignment { get; set; } = null!;

    public int InterventionStatusId { get; set; }
    public InterventionStatus InterventionStatus { get; set; } = null!;

    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InterventionMaterial> Materials { get; set; } = new List<InterventionMaterial>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}