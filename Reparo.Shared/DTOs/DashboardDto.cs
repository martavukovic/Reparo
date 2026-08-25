namespace Reparo.Shared.DTOs;

public class DashboardDto
{
    public int TotalOpen { get; set; }
    public int TotalCritical { get; set; }
    public int TotalOverdue { get; set; }
    public int TotalUnassigned { get; set; }
    public int TotalActiveInterventions { get; set; }
    public double AverageResolutionDays { get; set; }
    public List<FaultReportDto> LastFiveReports { get; set; } = new();
    public int MyReportsCount { get; set; }
    public int MyAssignmentsCount { get; set; }
}