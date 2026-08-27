using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")]
public class SlaController : ControllerBase
{
    private readonly AppDbContext _context;

    public SlaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<SlaDto>> Get()
    {
        var now = DateTime.UtcNow;

        var reports = await _context.FaultReports
            .Include(f => f.FaultStatus)
            .Include(f => f.FaultType)
            .Include(f => f.Location)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
            .Where(f => !f.IsDeleted)
            .ToListAsync();

        var closedStatusNames = new[] { "Resolved", "Closed" };

        var resolved = reports.Where(f =>
            closedStatusNames.Contains(f.FaultStatus.Name)).ToList();

        var overdue = reports.Where(f =>
            f.Deadline.HasValue &&
            f.Deadline < now &&
            !closedStatusNames.Contains(f.FaultStatus.Name)).ToList();

        // Prosječno rješavanje
        var resolutionDays = resolved
            .Select(f =>
            {
                var finished = f.Assignments
                    .SelectMany(a => a.Interventions)
                    .Where(i => i.FinishedAt.HasValue &&
                                i.InterventionStatus.Name == "Completed")
                    .Select(i => i.FinishedAt!.Value)
                    .FirstOrDefault();

                return finished == default
                    ? 0
                    : (finished - f.CreatedAt).TotalDays;
            })
            .Where(d => d > 0)
            .ToList();

        var avgResolution = resolutionDays.Any()
            ? Math.Round(resolutionDays.Average(), 1)
            : 0;

        // Po lokaciji
        var byLocation = reports
            .GroupBy(f => f.Location.Name)
            .Select(g => new SlaByLocationDto
            {
                LocationName = g.Key,
                TotalReports = g.Count(),
                ResolvedReports = g.Count(f =>
                    closedStatusNames.Contains(f.FaultStatus.Name)),
                AverageResolutionDays = Math.Round(
                    g.Where(f => closedStatusNames.Contains(f.FaultStatus.Name))
                     .Select(f =>
                     {
                         var finished = f.Assignments
                             .SelectMany(a => a.Interventions)
                             .Where(i => i.FinishedAt.HasValue &&
                                         i.InterventionStatus.Name == "Completed")
                             .Select(i => i.FinishedAt!.Value)
                             .FirstOrDefault();
                         return finished == default
                             ? 0
                             : (finished - f.CreatedAt).TotalDays;
                     })
                     .Where(d => d > 0)
                     .DefaultIfEmpty(0)
                     .Average(), 1)
            })
            .OrderByDescending(x => x.TotalReports)
            .ToList();

        // Po tipu
        var byType = reports
            .Where(f => f.FaultType != null)
            .GroupBy(f => f.FaultType!.Name)
            .Select(g => new SlaByTypeDto
            {
                FaultTypeName = g.Key,
                TotalReports = g.Count(),
                ResolvedReports = g.Count(f =>
                    closedStatusNames.Contains(f.FaultStatus.Name)),
                AverageResolutionDays = Math.Round(
                    g.Where(f => closedStatusNames.Contains(f.FaultStatus.Name))
                     .Select(f =>
                     {
                         var finished = f.Assignments
                             .SelectMany(a => a.Interventions)
                             .Where(i => i.FinishedAt.HasValue &&
                                         i.InterventionStatus.Name == "Completed")
                             .Select(i => i.FinishedAt!.Value)
                             .FirstOrDefault();
                         return finished == default
                             ? 0
                             : (finished - f.CreatedAt).TotalDays;
                     })
                     .Where(d => d > 0)
                     .DefaultIfEmpty(0)
                     .Average(), 1)
            })
            .OrderByDescending(x => x.TotalReports)
            .ToList();

        return Ok(new SlaDto
        {
            TotalReports = reports.Count,
            ResolvedReports = resolved.Count,
            OverdueReports = overdue.Count,
            AverageResolutionDays = avgResolution,
            ResolutionRate = reports.Count > 0
                ? Math.Round((double)resolved.Count / reports.Count * 100, 1)
                : 0,
            ByLocation = byLocation,
            ByType = byType
        });
    }
}