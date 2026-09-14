using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Models;
using Reparo.Shared.Models;

namespace Reparo.Api.Data;

public static class SeedData
{
    public static async Task SeedUsersAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext context)
    {
        string[] roles = { "Admin", "Manager", "Technician", "Reporter" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var employeesToAdd = new List<Employee>
        {
            new() { Id = 6, FirstName = "Ana", LastName = "Kovač",
                    LocationId = 1, IsTechnician = false,
                    IsActive = true, IsAvailable = true },
            new() { Id = 7, FirstName = "Marko", LastName = "Horvat",
                    LocationId = 2, IsTechnician = true,
                    IsActive = true, IsAvailable = true },
            new() { Id = 8, FirstName = "Petra", LastName = "Novak",
                    LocationId = 3, IsTechnician = true,
                    IsActive = true, IsAvailable = true },
            new() { Id = 9, FirstName = "Ivan", LastName = "Babić",
                    LocationId = 4, IsTechnician = false,
                    IsActive = true, IsAvailable = true },
            new() { Id = 10, FirstName = "Maja", LastName = "Jurić",
                    LocationId = 2, IsTechnician = true,
                    IsActive = true, IsAvailable = true },
            new() { Id = 11, FirstName = "Luka", LastName = "Matić",
                    LocationId = 2, IsTechnician = false,
                    IsActive = true, IsAvailable = true },
                    };

        foreach (var emp in employeesToAdd)
        {
            if (!await context.Employees.AnyAsync(e => e.Id == emp.Id))
                context.Employees.Add(emp);
        }
        await context.SaveChangesAsync();

        if (await userManager.FindByEmailAsync("admin@reparo.local") is null)
        {
            var admin = new AppUser
            {
                UserName = "admin@reparo.local",
                Email = "admin@reparo.local"
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Manager" });
        }

        if (await userManager.FindByEmailAsync("manager@reparo.local") is null)
        {
            var manager = new AppUser
            {
                UserName = "manager@reparo.local",
                Email = "manager@reparo.local"
            };
            await userManager.CreateAsync(manager, "Manager123!");
            await userManager.AddToRoleAsync(manager, "Manager");
        }

        if (await userManager.FindByEmailAsync("tech@reparo.local") is null)
        {
            var tech = new AppUser
            {
                UserName = "tech@reparo.local",
                Email = "tech@reparo.local",
                EmployeeId = 3
            };
            await userManager.CreateAsync(tech, "Tech123!");
            await userManager.AddToRoleAsync(tech, "Technician");
        }

        if (await userManager.FindByEmailAsync("reporter@reparo.local") is null)
        {
            var reporter = new AppUser
            {
                UserName = "reporter@reparo.local",
                Email = "reporter@reparo.local",
                EmployeeId = 1
            };
            await userManager.CreateAsync(reporter, "Reporter123!");
            await userManager.AddToRoleAsync(reporter, "Reporter");
        }

        if (await userManager.FindByEmailAsync("ana@reparo.local") is null)
        {
            var ana = new AppUser
            {
                UserName = "ana@reparo.local",
                Email = "ana@reparo.local",
                EmployeeId = 6
            };
            await userManager.CreateAsync(ana, "Ana123!");
            await userManager.AddToRoleAsync(ana, "Reporter");
        }

        if (await userManager.FindByEmailAsync("marko@reparo.local") is null)
        {
            var marko = new AppUser
            {
                UserName = "marko@reparo.local",
                Email = "marko@reparo.local",
                EmployeeId = 7
            };
            await userManager.CreateAsync(marko, "Marko123!");
            await userManager.AddToRoleAsync(marko, "Technician");
        }

        if (await userManager.FindByEmailAsync("petra@reparo.local") is null)
        {
            var petra = new AppUser
            {
                UserName = "petra@reparo.local",
                Email = "petra@reparo.local",
                EmployeeId = 8
            };
            await userManager.CreateAsync(petra, "Petra123!");
            await userManager.AddToRoleAsync(petra, "Technician");
        }

        if (await userManager.FindByEmailAsync("ivan@reparo.local") is null)
        {
            var ivan = new AppUser
            {
                UserName = "ivan@reparo.local",
                Email = "ivan@reparo.local",
                EmployeeId = 9
            };
            await userManager.CreateAsync(ivan, "Ivan123!");
            await userManager.AddToRoleAsync(ivan, "Reporter");
        }

        if (await userManager.FindByEmailAsync("maja@reparo.local") is null)
        {
            var maja = new AppUser
            {
                UserName = "maja@reparo.local",
                Email = "maja@reparo.local",
                EmployeeId = 10
            };
            await userManager.CreateAsync(maja, "Maja123!");
            await userManager.AddToRoleAsync(maja, "Technician");
        }

        if (await userManager.FindByEmailAsync("luka@reparo.local") is null)
        {
            var luka = new AppUser
            {
                UserName = "luka@reparo.local",
                Email = "luka@reparo.local",
                EmployeeId = 11
            };
            await userManager.CreateAsync(luka, "Luka123!");
            await userManager.AddToRoleAsync(luka, "Reporter");
        }
    }
}