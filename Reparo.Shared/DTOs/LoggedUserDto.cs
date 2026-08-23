namespace Reparo.Shared.DTOs;

public class LoggedUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int? EmployeeId { get; set; }
    public string Token { get; set; } = string.Empty;
}