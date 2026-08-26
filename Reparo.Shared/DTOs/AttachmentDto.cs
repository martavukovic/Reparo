namespace Reparo.Shared.DTOs;

public class AttachmentDto
{
    public int Id { get; set; }
    public int FaultReportId { get; set; }
    public int? InterventionId { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Url { get; set; } = string.Empty;
}