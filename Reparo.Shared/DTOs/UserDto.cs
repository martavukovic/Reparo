namespace Reparo.Shared.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public int? EmployeeId { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsTechnician => Role == "Technician";
}