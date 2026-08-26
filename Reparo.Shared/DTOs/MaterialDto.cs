namespace Reparo.Shared.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class MaterialCreateDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class InterventionMaterialDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int MaterialUnitId { get; set; }
    public string MaterialUnitName { get; set; } = string.Empty;
}

public class InterventionMaterialCreateDto
{
    public int InterventionId { get; set; }
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }
    public int MaterialUnitId { get; set; }
}

public class CreateMaterialDto
{
    public string Name { get; set; } = string.Empty;
}