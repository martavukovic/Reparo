namespace Reparo.Shared.DTOs;

public class FaultReportFilterDto
{
    public string? SearchText { get; set; }
    public int? LocationId { get; set; }
    public int? FaultTypeId { get; set; }
    public int? FaultPriorityId { get; set; }
    public int? FaultStatusId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}