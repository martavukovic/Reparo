namespace Reparo.Shared.DTOs;

public class SlaDto
{
    public int TotalReports { get; set; }
    public int ResolvedReports { get; set; }
    public int OverdueReports { get; set; }
    public double AverageResolutionDays { get; set; }
    public double ResolutionRate { get; set; }
    public List<SlaByLocationDto> ByLocation { get; set; } = new();
    public List<SlaByTypeDto> ByType { get; set; } = new();
}

public class SlaByLocationDto
{
    public string LocationName { get; set; } = string.Empty;
    public int TotalReports { get; set; }
    public int ResolvedReports { get; set; }
    public double AverageResolutionDays { get; set; }
}

public class SlaByTypeDto
{
    public string FaultTypeName { get; set; } = string.Empty;
    public int TotalReports { get; set; }
    public int ResolvedReports { get; set; }
    public double AverageResolutionDays { get; set; }
}