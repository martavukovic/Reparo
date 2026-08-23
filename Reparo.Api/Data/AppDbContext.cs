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

        // FaultReport - dva FK prema Employee, EF treba znati koji je koji
        builder.Entity<FaultReport>()
            .HasOne(f => f.ReportedByEmployee)
            .WithMany(e => e.ReportedFaults)
            .HasForeignKey(f => f.ReportedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FaultReport>()
            .HasOne(f => f.Location)
            .WithMany(l => l.FaultReports)
            .HasForeignKey(f => f.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // WorkAssignment - FK prema Employee kao Technician
        builder.Entity<WorkAssignment>()
            .HasOne(w => w.Technician)
            .WithMany(e => e.Assignments)
            .HasForeignKey(w => w.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);

        // InterventionMaterial - composite key nije potreban, Id je dovoljan
        builder.Entity<InterventionMaterial>()
            .Property(m => m.Quantity)
            .HasColumnType("decimal(10,2)");

        // Seed lookup podatci
        builder.Entity<LocationType>().HasData(
            new LocationType { Id = 1, Name = "Upravna zgrada" },
            new LocationType { Id = 2, Name = "Škola" },
            new LocationType { Id = 3, Name = "Zdravstvena ustanova" },
            new LocationType { Id = 4, Name = "Skladište" }
        );

        builder.Entity<FaultType>().HasData(
            new FaultType { Id = 1, Name = "Elektrika" },
            new FaultType { Id = 2, Name = "Voda" },
            new FaultType { Id = 3, Name = "Grijanje" },
            new FaultType { Id = 4, Name = "Mreža" },
            new FaultType { Id = 5, Name = "Građevinski radovi" },
            new FaultType { Id = 6, Name = "Ostalo" }
        );

        builder.Entity<FaultPriority>().HasData(
            new FaultPriority { Id = 1, Name = "Nizak" },
            new FaultPriority { Id = 2, Name = "Srednji" },
            new FaultPriority { Id = 3, Name = "Visok" },
            new FaultPriority { Id = 4, Name = "Kritičan" }
        );

        builder.Entity<FaultStatus>().HasData(
            new FaultStatus { Id = 1, Name = "Zaprimljeno" },
            new FaultStatus { Id = 2, Name = "Pregledano" },
            new FaultStatus { Id = 3, Name = "Dodijeljeno" },
            new FaultStatus { Id = 4, Name = "U radu" },
            new FaultStatus { Id = 5, Name = "Riješeno" },
            new FaultStatus { Id = 6, Name = "Zatvoreno" }
        );

        builder.Entity<InterventionStatus>().HasData(
            new InterventionStatus { Id = 1, Name = "Planirana" },
            new InterventionStatus { Id = 2, Name = "U tijeku" },
            new InterventionStatus { Id = 3, Name = "Završena" },
            new InterventionStatus { Id = 4, Name = "Neuspješna" }
        );

        builder.Entity<MaterialUnit>().HasData(
            new MaterialUnit { Id = 1, Name = "Komad" },
            new MaterialUnit { Id = 2, Name = "Metar" },
            new MaterialUnit { Id = 3, Name = "Litra" },
            new MaterialUnit { Id = 4, Name = "Kilogram" },
            new MaterialUnit { Id = 5, Name = "Paket" }
        );
    }
}