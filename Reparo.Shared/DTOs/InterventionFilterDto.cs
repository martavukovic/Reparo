namespace Reparo.Shared.DTOs;

public class InterventionFilterDto
{
    public string? SearchText { get; set; }
    public int? StatusId { get; set; }
    public int? TechnicianId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}