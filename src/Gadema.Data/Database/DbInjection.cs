using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Gadema.Data.Database;

namespace Gadema.Data.Database;

public static class DbInjection
{
    public static IServiceCollection AddGademaData(this IServiceCollection services, IConfiguration configuration)
    {
        // The implementation detail (Sqlite) is hidden inside this method
        services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}