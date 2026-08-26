using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Models;
using Reparo.Shared.Models;

namespace Reparo.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<LocationType> LocationTypes => Set<LocationType>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<FaultReport> FaultReports => Set<FaultReport>();
    public DbSet<FaultType> FaultTypes => Set<FaultType>();
    public DbSet<FaultPriority> FaultPriorities => Set<FaultPriority>();
    public DbSet<FaultStatus> FaultStatuses => Set<FaultStatus>();
    public DbSet<WorkAssignment> WorkAssignments => Set<WorkAssignment>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<InterventionStatus> InterventionStatuses => Set<InterventionStatus>();
    public DbSet<InterventionMaterial> InterventionMaterials => Set<InterventionMaterial>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialUnit> MaterialUnits => Set<MaterialUnit>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LocationType>().HasData(
            new LocationType { Id = 1, Name = "Administrative Building" },
            new LocationType { Id = 2, Name = "School" },
            new LocationType { Id = 3, Name = "Healthcare Facility" },
            new LocationType { Id = 4, Name = "Warehouse" }
        );

        builder.Entity<FaultType>().HasData(
            new FaultType { Id = 1, Name = "Electrical" },
            new FaultType { Id = 2, Name = "Plumbing" },
            new FaultType { Id = 3, Name = "Heating" },
            new FaultType { Id = 4, Name = "Network" },
            new FaultType { Id = 5, Name = "Construction" },
            new FaultType { Id = 6, Name = "Other" }
        );

        builder.Entity<FaultPriority>().HasData(
            new FaultPriority { Id = 1, Name = "Low" },
            new FaultPriority { Id = 2, Name = "Medium" },
            new FaultPriority { Id = 3, Name = "High" },
            new FaultPriority { Id = 4, Name = "Critical" }
        );

        builder.Entity<FaultStatus>().HasData(
            new FaultStatus { Id = 1, Name = "Submitted" },
            new FaultStatus { Id = 2, Name = "Reviewed" },
            new FaultStatus { Id = 3, Name = "Assigned" },
            new FaultStatus { Id = 4, Name = "In Progress" },
            new FaultStatus { Id = 5, Name = "Resolved" },
            new FaultStatus { Id = 6, Name = "Closed" }
        );

        builder.Entity<InterventionStatus>().HasData(
            new InterventionStatus { Id = 1, Name = "Planned" },
            new InterventionStatus { Id = 2, Name = "In Progress" },
            new InterventionStatus { Id = 3, Name = "Completed" },
            new InterventionStatus { Id = 4, Name = "Failed" }
        );

        builder.Entity<MaterialUnit>().HasData(
            new MaterialUnit { Id = 1, Name = "Piece" },
            new MaterialUnit { Id = 2, Name = "Meter" },
            new MaterialUnit { Id = 3, Name = "Liter" },
            new MaterialUnit { Id = 4, Name = "Kilogram" },
            new MaterialUnit { Id = 5, Name = "Package" }
        );

        builder.Entity<Location>().HasData(
            new Location { Id = 1, Name = "County Hall", Address = "Main Street 1", LocationTypeId = 1, IsActive = true },
            new Location { Id = 2, Name = "Central School", Address = "School Avenue 5", LocationTypeId = 2, IsActive = true },
            new Location { Id = 3, Name = "General Hospital", Address = "Hospital Road 10", LocationTypeId = 3, IsActive = true },
            new Location { Id = 4, Name = "Main Warehouse", Address = "Industrial Zone 3", LocationTypeId = 4, IsActive = true }
        );

        builder.Entity<Employee>().HasData(
            new Employee { Id = 1, FirstName = "John", LastName = "Smith", LocationId = 1, IsTechnician = false, IsActive = true },
            new Employee { Id = 2, FirstName = "Sarah", LastName = "Johnson", LocationId = 2, IsTechnician = false, IsActive = true },
            new Employee { Id = 3, FirstName = "Mike", LastName = "Williams", LocationId = 1, IsTechnician = true, IsActive = true },
            new Employee { Id = 4, FirstName = "Tom", LastName = "Brown", LocationId = 3, IsTechnician = true, IsActive = true },
            new Employee { Id = 5, FirstName = "Emma", LastName = "Davis", LocationId = 4, IsTechnician = false, IsActive = true }
        );

        builder.Entity<Material>().HasData(
            new Material { Id = 1, Name = "Cable 2.5mm", IsActive = true },
            new Material { Id = 2, Name = "Light Bulb LED", IsActive = true },
            new Material { Id = 3, Name = "PVC Pipe 20mm", IsActive = true },
            new Material { Id = 4, Name = "Sealant", IsActive = true },
            new Material { Id = 5, Name = "Screw Set", IsActive = true }
        );
    }
}