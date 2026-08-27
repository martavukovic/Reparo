namespace Reparo.Shared.Services;

public interface IAiService
{
    string ProviderName { get; }
    bool UsesExternalService { get; }
    Task<string> GenerateTextAsync(string purpose, string input);
    Task<T?> GenerateStructuredAsync<T>(string purpose, string input);
}