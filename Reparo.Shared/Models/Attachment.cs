namespace Reparo.Shared.Models;

public class Attachment
{
    public int Id { get; set; }

    public int FaultReportId { get; set; }
    public FaultReport FaultReport { get; set; } = null!;

    public int? InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public string Purpose { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedByUserId { get; set; } = string.Empty;
}