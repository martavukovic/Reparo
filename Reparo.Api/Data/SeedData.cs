using Microsoft.AspNetCore.Identity;
using Reparo.Api.Models;

namespace Reparo.Api.Data;

public static class SeedData
{
    public static async Task SeedUsersAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed uloga
        string[] roles = { "Admin", "Manager", "Technician", "Reporter" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin korisnika
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

        // Seed manager korisnika
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

        // Seed technician korisnika
        if (await userManager.FindByEmailAsync("tech@reparo.local") is null)
        {
            var tech = new AppUser
            {
                UserName = "tech@reparo.local",
                Email = "tech@reparo.local"
            };

            await userManager.CreateAsync(tech, "Tech123!");
            await userManager.AddToRoleAsync(tech, "Technician");
        }

        // Seed reporter korisnika
        if (await userManager.FindByEmailAsync("reporter@reparo.local") is null)
        {
            var reporter = new AppUser
            {
                UserName = "reporter@reparo.local",
                Email = "reporter@reparo.local"
            };

            await userManager.CreateAsync(reporter, "Reporter123!");
            await userManager.AddToRoleAsync(reporter, "Reporter");
        }
    }
}