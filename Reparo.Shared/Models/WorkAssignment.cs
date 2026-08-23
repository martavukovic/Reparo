namespace Reparo.Shared.Models;

public class WorkAssignment
{
    public int Id { get; set; }

    public int FaultReportId { get; set; }
    public FaultReport FaultReport { get; set; } = null!;

    public int TechnicianId { get; set; }
    public Employee Technician { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string AssignedByUserId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? Note { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();
}