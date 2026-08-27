namespace Reparo.Shared.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public bool IsTechnician { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;

    public ICollection<FaultReport> ReportedFaults { get; set; } = new List<FaultReport>();
    public ICollection<WorkAssignment> Assignments { get; set; } = new List<WorkAssignment>();
}