namespace Reparo.Shared.DTOs;

public class AiSuggestionResult
{
    public string SuggestedTitle { get; set; } = string.Empty;
    public string SuggestedFaultType { get; set; } = string.Empty;
    public string SuggestedPriority { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; } = false;
}

public class AiProviderDto
{
    public string Provider { get; set; } = string.Empty;
    public bool UsesExternalService { get; set; }
}