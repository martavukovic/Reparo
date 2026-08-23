namespace Reparo.Shared.Models;

public class Material
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<InterventionMaterial> InterventionMaterials { get; set; } = new List<InterventionMaterial>();
}