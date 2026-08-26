using MudBlazor.Services;
using Reparo.App.Components;
using Reparo.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddSingleton<CurrentUserService>();
builder.Services.AddSingleton<AuthHeaderHandler>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddHttpClient("ReparoApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7182/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("ReparoApi"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();