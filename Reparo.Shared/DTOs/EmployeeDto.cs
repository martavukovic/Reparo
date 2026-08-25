namespace Reparo.Shared.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public bool IsTechnician { get; set; }
    public bool IsActive { get; set; }
}

public class EmployeeCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public bool IsTechnician { get; set; }
    public bool IsActive { get; set; } = true;
}