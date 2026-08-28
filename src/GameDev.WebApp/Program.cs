using GameDev.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

// Map Razor components with MudBlazor integration
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    //.AddAdditionalAssemblies(typeof(GameDev.WebApp.Components._Imports).Assembly);

// Configure health check endpoint (optional, for monitoring)
//app.MapHealthChecks("/health");

app.Run();
