namespace Reparo.Shared.DTOs;

public class InterventionListDto
{
    public int Id { get; set; }
    public int WorkAssignmentId { get; set; }
    public int FaultReportId { get; set; }
    public string FaultReportTitle { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public int InterventionStatusId { get; set; }
    public string InterventionStatusName { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Note { get; set; }
    public int MaterialCount { get; set; }
    public DateTime CreatedAt { get; set; }
}