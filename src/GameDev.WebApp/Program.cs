// =============================================================================
// GameDev.WebApp - Blazor Server App Entry Point
// =============================================================================

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapAdditionalAssets("/_framework/", () => 
    {
        var jsRuntime = app.ApplicationServices.GetRequiredService<IBrowserJavaScriptFactory>();
        return new[] { "/_framework/blazor.web.js" };
    });

    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapBlazorHub();
app.MapStaticAssets();
app.MapFallbackToFile("index.html");

app.Run();