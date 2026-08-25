using System.Net.Http.Json;
using Reparo.Shared.DTOs;

namespace Reparo.App.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly CurrentUserService _currentUser;
    public string? CurrentToken { get; private set; }

    public AuthService(HttpClient http, CurrentUserService currentUser)
    {
        _http = http;
        _currentUser = currentUser;
    }

    public async Task<bool> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return false;

            var user = await response.Content.ReadFromJsonAsync<LoggedUserDto>();

            if (user is null)
                return false;

            _currentUser.Login(user);
            CurrentToken = user.Token;

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", user.Token);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Logout()
    {
        _currentUser.Logout();
        _http.DefaultRequestHeaders.Authorization = null;
    }
}