namespace Reparo.Shared.DTOs;

public class WorkAssignmentDto
{
    public int Id { get; set; }
    public int FaultReportId { get; set; }
    public string FaultReportTitle { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string FaultTypeName { get; set; } = string.Empty;
    public string FaultPriorityName { get; set; } = string.Empty;
    public string FaultStatusName { get; set; } = string.Empty;
    public int TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string? AssignedByName { get; set; }
    public bool IsActive { get; set; }
    public string? Note { get; set; }
    public bool HasFailedIntervention { get; set; }
    public List<InterventionDto> Interventions { get; set; } = new();
}

public class WorkAssignmentCreateDto
{
    public int FaultReportId { get; set; }
    public int TechnicianId { get; set; }
    public string? Note { get; set; }
}