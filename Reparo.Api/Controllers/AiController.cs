using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reparo.Shared.DTOs;
using Reparo.Shared.Services;

namespace Reparo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
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
        var summary = await _aiService.GenerateTextAsync(
            "fault-summary", $"Fault report ID: {faultReportId}");

        return Ok(summary);
    }
}