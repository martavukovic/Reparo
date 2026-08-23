namespace Reparo.Shared.Models;

public class InterventionMaterial
{
    public int Id { get; set; }

    public int InterventionId { get; set; }
    public Intervention Intervention { get; set; } = null!;

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public decimal Quantity { get; set; }

    public int MaterialUnitId { get; set; }
    public MaterialUnit MaterialUnit { get; set; } = null!;
}