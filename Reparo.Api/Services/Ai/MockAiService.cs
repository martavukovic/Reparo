using System.Text.Json;
using Reparo.Shared.DTOs;
using Reparo.Shared.Services;

namespace Reparo.Api.Services.Ai;

public sealed class MockAiService : IAiService
{
    public string ProviderName => "Mock";
    public bool UsesExternalService => false;

    public Task<string> GenerateTextAsync(string purpose, string input)
    {
        var result = purpose switch
        {
            "fault-summary" => GenerateFaultSummary(input),
            _ => $"[Mock AI] Response for: {input}"
        };

        return Task.FromResult(result);
    }

    public Task<T?> GenerateStructuredAsync<T>(string purpose, string input)
    {
        if (purpose == "fault-suggest" && typeof(T) == typeof(AiSuggestionResult))
        {
            var suggestion = GenerateFaultSuggestion(input);
            var json = JsonSerializer.Serialize(suggestion);
            var result = JsonSerializer.Deserialize<T>(json);
            return Task.FromResult(result);
        }

        return Task.FromResult(default(T));
    }

    private static string GenerateFaultSummary(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string Get(string key) => lines
            .FirstOrDefault(l => l.TrimStart().StartsWith(key))?
            .Split(':', 2).LastOrDefault()?.Trim() ?? "-";

        var title = Get("Title");
        var location = Get("Location");
        var priority = Get("Priority");
        var status = Get("Status");
        var type = Get("Type");
        var deadline = Get("Deadline");
        var interventions = Get("Interventions");
        var materials = Get("Materials used");

        var urgency = priority == "Critical" ? "urgent " : priority == "High" ? "high-priority " : "";

        return $"[Mock AI] The {urgency}fault report \"{title}\" was submitted for {location}. " +
               $"Fault type: {type}, current status: {status}." +
               (deadline != "None" ? $" Deadline: {deadline}." : "") +
               $" Interventions: {interventions}." +
               (materials != "None" ? $" Materials used: {materials}." : " No materials used.");
    }

    private static AiSuggestionResult GenerateFaultSuggestion(string title)
    {
        var t = title.ToLower();

        var result = new AiSuggestionResult
        {
            SuggestedTitle = CapitalizeFirst(title.Trim()),
            SuggestedFaultType = "Other",
            SuggestedPriority = "Low",
            Reasoning = "Could not determine fault type from title."
        };

        // Tip kvara
        if (ContainsAny(t, "light", "bulb", "electric", "power", "socket",
            "fuse", "circuit", "switch", "wiring", "voltage", "blackout"))
        {
            result.SuggestedFaultType = "Electrical";
            result.SuggestedPriority = "Medium";
            result.Reasoning = "Electrical keywords detected.";
        }
        else if (ContainsAny(t, "water", "pipe", "leak", "drain", "sink",
            "toilet", "shower", "tap", "faucet", "valve", "burst", "sewage"))
        {
            result.SuggestedFaultType = "Plumbing";
            result.SuggestedPriority = "High";
            result.Reasoning = "Plumbing keywords detected. Water issues can escalate quickly.";
        }
        else if (ContainsAny(t, "heat", "boiler", "radiator", "cold",
            "thermostat", "hvac", "ventilation", "air", "temperature", "freezing"))
        {
            result.SuggestedFaultType = "Heating";
            result.SuggestedPriority = "High";
            result.Reasoning = "Heating keywords detected.";
        }
        else if (ContainsAny(t, "network", "internet", "wifi", "wi-fi",
            "router", "ethernet", "lan", "connection", "signal", "wireless"))
        {
            result.SuggestedFaultType = "Network";
            result.SuggestedPriority = "Low";
            result.Reasoning = "Network keywords detected.";
        }
        else if (ContainsAny(t, "wall", "floor", "roof", "ceiling", "door",
            "window", "crack", "broken", "paint", "tile", "stairs", "elevator",
            "lift", "lock", "handle", "structure", "foundation"))
        {
            result.SuggestedFaultType = "Construction";
            result.SuggestedPriority = "Low";
            result.Reasoning = "Construction keywords detected.";
        }

        // Kritični override
        if (ContainsAny(t, "fire", "smoke", "gas", "explosion", "collapse",
            "chemical", "urgent", "emergency", "critical", "danger",
            "hazard", "toxic", "injury", "flood", "mold", "structural"))
        {
            result.SuggestedPriority = "Critical";
            result.Reasoning += " Dangerous keywords detected — Critical priority assigned.";
        }
        else if (ContainsAny(t, "no power", "no water", "no heat", "no internet",
            "not working", "failed", "major", "serious", "smell", "loud noise"))
        {
            if (result.SuggestedPriority == "Low")
                result.SuggestedPriority = "High";
            result.Reasoning += " Severity keywords detected.";
        }

        return result;
    }

    private static bool ContainsAny(string text, params string[] keywords)
        => keywords.Any(text.Contains);

    private static string CapitalizeFirst(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        return char.ToUpper(text[0]) + text[1..];
    }
}