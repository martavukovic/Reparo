namespace Reparo.App.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly CurrentUserService _currentUser;

    public AuthHeaderHandler(CurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handler called. User: {_currentUser.User?.Email}, Token: {(_currentUser.User?.Token is not null ? "EXISTS" : "NULL")}");

        if (_currentUser.User?.Token is not null)
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", _currentUser.User.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}