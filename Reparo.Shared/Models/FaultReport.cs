using System.Net.Mail;

namespace Reparo.Shared.Models;

public class FaultReport
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int ReportedByEmployeeId { get; set; }
    public Employee ReportedByEmployee { get; set; } = null!;

    public int? FaultTypeId { get; set; }
    public FaultType? FaultType { get; set; }

    public int? FaultPriorityId { get; set; }
    public FaultPriority? FaultPriority { get; set; }

    public int FaultStatusId { get; set; }
    public FaultStatus FaultStatus { get; set; } = null!;

    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public string? AiSummary { get; set; }
    public DateTime? AiSummaryGeneratedAt { get; set; }
    public ICollection<WorkAssignment> Assignments { get; set; } = new List<WorkAssignment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}