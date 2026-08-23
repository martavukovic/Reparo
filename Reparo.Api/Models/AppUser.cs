using Microsoft.AspNetCore.Identity;
using Reparo.Shared.Models;

namespace Reparo.Api.Models;

public class AppUser : IdentityUser
{
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}