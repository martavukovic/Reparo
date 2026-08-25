namespace Reparo.Shared.DTOs;

public class InterventionDto
{
    public int Id { get; set; }
    public int WorkAssignmentId { get; set; }
    public int InterventionStatusId { get; set; }
    public string InterventionStatusName { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InterventionMaterialDto> Materials { get; set; } = new();
}

public class InterventionCreateDto
{
    public int WorkAssignmentId { get; set; }
}

public class InterventionFinishDto
{
    public int Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
}