using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reparo.Api.Data;
using Reparo.Shared.DTOs;
using Reparo.Shared.Services;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly AppDbContext _context;

    public AiController(IAiService aiService, AppDbContext context)
    {
        _aiService = aiService;
        _context = context;
    }

    [HttpGet("provider")]
    public ActionResult<object> GetProvider()
    {
        return Ok(new
        {
            Provider = _aiService.ProviderName,
            UsesExternalService = _aiService.UsesExternalService
        });
    }

    [HttpPost("suggest")]
    public async Task<ActionResult<AiSuggestionResult>> Suggest(
        [FromBody] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        var result = await _aiService.GenerateStructuredAsync<AiSuggestionResult>(
            "fault-suggest", title);

        return Ok(result ?? new AiSuggestionResult
        {
            SuggestedFaultType = "Other",
            SuggestedPriority = "Low",
            Reasoning = "Could not generate suggestion."
        });
    }

    [HttpGet("summarize/{faultReportId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<string>> Summarize(int faultReportId)
    {
        var report = await _context.FaultReports
            .Include(f => f.Location)
            .Include(f => f.FaultType)
            .Include(f => f.FaultPriority)
            .Include(f => f.FaultStatus)
            .Include(f => f.ReportedByEmployee)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Technician)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.InterventionStatus)
            .Include(f => f.Assignments)
                .ThenInclude(a => a.Interventions)
                    .ThenInclude(i => i.Materials)
                        .ThenInclude(m => m.Material)
            .FirstOrDefaultAsync(f => f.Id == faultReportId);

        if (report is null)
            return NotFound();

        var interventions = report.Assignments
            .SelectMany(a => a.Interventions)
            .ToList();

        var materials = interventions
            .SelectMany(i => i.Materials)
            .Select(m => $"{m.Material?.Name} x{m.Quantity}")
            .ToList();

        var input = $"""
        Title: {report.Title}
        Description: {report.Description}
        Location: {report.Location?.Name}
        Reported by: {report.ReportedByEmployee?.FirstName} {report.ReportedByEmployee?.LastName}
        Type: {report.FaultType?.Name ?? "Unknown"}
        Priority: {report.FaultPriority?.Name ?? "Unknown"}
        Status: {report.FaultStatus?.Name}
        Created: {report.CreatedAt:dd.MM.yyyy}
        Deadline: {(report.Deadline.HasValue ? report.Deadline.Value.ToString("dd.MM.yyyy") : "None")}
        Interventions: {interventions.Count} total, {interventions.Count(i => i.InterventionStatus?.Name == "Completed")} completed, {interventions.Count(i => i.InterventionStatus?.Name == "Failed")} failed
        Materials used: {(materials.Any() ? string.Join(", ", materials) : "None")}
        """;

        var summary = await _aiService.GenerateTextAsync("fault-summary", input);
        return Ok(summary);
    }
}