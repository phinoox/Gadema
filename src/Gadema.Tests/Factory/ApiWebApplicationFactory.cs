using Gadema.Api;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Test host for Gadema.Api.
/// - Runs Program.Main (all services registered exactly as in production)
/// - Overrides config with test values (in-memory SQLite, valid JWT keys)
/// - Shares ONE open SqliteConnection across all scopes = one persistent DB
/// - Keeps a long-lived scope so resolved services stay alive for the whole test class
/// </summary>
public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    private readonly StreamWriter _logWriter = new StreamWriter("unit_test_db.log",append:false);
    private IServiceScope? _scope;

    private static bool _FkChecked = false;

    public ApiWebApplicationFactory()
    {
        // Must stay OPEN for the factory's lifetime — closing it drops the in-memory DB.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        using var pragmaCmd = _connection.CreateCommand();
        pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;PRAGMA foreign_key_check;";
        pragmaCmd.ExecuteNonQuery();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Test overrides for appsettings.json (in-memory DB + valid JWT secret).
        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",
                ["JwtSecret"]           = new string('T', 64),
                ["PasswordSalt"]        = "s3cur3p@ssw0rdS4lt!",
                ["2FAValidatingHash"]   = "s3cur3t0pH@sh!",
                ["Jwt:Secret"]          = new string('J', 64), // ≥32 bytes, required by HS256
                ["Jwt:Issuer"]          = "GaDeMa",
                ["Jwt:Audience"]        = "GaDeMaApi",
                ["GoogleOauth:ClientId"]     = "test-client-id.apps.googleusercontent.com",
                ["GoogleOauth:ClientSecret"] = "test-client-secret"
            }));

        builder.ConfigureServices(services =>
        {
            // Drop Program.cs's DbContext registration, re-register on the shared connection.
            services.RemoveAll<DbContextOptions<GameDbContext>>();
            services.RemoveAll<GameDbContext>();

            services.AddDbContext<GameDbContext>(
                o => o.UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .LogTo(_logWriter.WriteLine, LogLevel.Information));
            services.AddSingleton(_connection); // prevent DI from disposing it per scope
        });
        
        
    }

    /// <summary>
    /// Long-lived scope (created once, disposed only with the factory).
    /// Services resolved here stay valid for the entire test class lifetime —
    /// this is what makes constructor-resolved services like _emailAuth work.
    /// </summary>
    private IServiceScope GetOrCreateScope()
        => _scope ??= Services.CreateScope();

    public T GetScopedService<T>() where T : notnull
        => GetOrCreateScope().ServiceProvider.GetRequiredService<T>();

    /// <summary>Drop + recreate schema — call between tests for isolation.</summary>
    public void ResetDb()
    {
        var db = GetOrCreateScope().ServiceProvider.GetRequiredService<GameDbContext>();
        db.Database.EnsureDeleted();
        //db.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
        List<string> db_pragma = new List<string>(){
            "foreign_key_check",
            "defer_foreign_keys=true",
            "main.quick_check"
            };
        foreach (var pragma in db_pragma)
        {
            db.Database.ExecuteSqlRaw($"PRAGMA {pragma};");
        }
        db.Database.EnsureCreated();
        foreach (var pragma in db_pragma)
        {
            db.Database.ExecuteSqlRaw($"PRAGMA {pragma};");
        }

    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Ensure schema exists before any request is served.
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            if (!db.Database.EnsureCreated())
                throw new InvalidOperationException("Failed to create test database.");
        }

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _scope?.Dispose();   // disposes the scoped DbContext first…
            _connection.Dispose(); // …then closes the shared connection
        }
        base.Dispose(disposing);
        _logWriter.Dispose();
    }

    private void CheckFK()
    {
        if(_FkChecked == true)
        return;
        _FkChecked = true;
        using var command = _connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;PRAGMA foreign_key_check;";
       // _context.Database.OpenConnection();
       command.ExecuteNonQuery();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            // Each row represents a violation.
            // Columns typically include: table_name, row_id, parent_table_name, foreign_key_index
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine($"fk violation: {reader.GetName(i)}: {reader.GetValue(i)}");
            }
        }
       // _context.Database.CloseConnection();
    }

    internal IServiceScope GetScope()
    {
       return _scope;
    }
}