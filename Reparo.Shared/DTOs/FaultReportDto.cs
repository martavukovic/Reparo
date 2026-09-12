namespace Reparo.Shared.DTOs;

public class FaultReportDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int ReportedByEmployeeId { get; set; }
    public string ReportedByEmployeeName { get; set; } = string.Empty;
    public int? FaultTypeId { get; set; }
    public string? FaultTypeName { get; set; }
    public int? FaultPriorityId { get; set; }
    public string? FaultPriorityName { get; set; }
    public int FaultStatusId { get; set; }
    public string FaultStatusName { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool HasFailedIntervention { get; set; }
    public string? ActiveTechnicianName { get; set; }
    public string? AiSummary { get; set; }
    public DateTime? AiSummaryGeneratedAt { get; set; }
}

public class FaultReportCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public int ReportedByEmployeeId { get; set; }
    public int? FaultTypeId { get; set; }
}

public class FaultReportUpdateDto
{
    public int? FaultTypeId { get; set; }
    public int? FaultPriorityId { get; set; }
    public DateTime? Deadline { get; set; }
}