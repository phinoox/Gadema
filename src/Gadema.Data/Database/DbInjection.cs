using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Gadema.Data.Database;
using Gadema.Data.Database.Game;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Gadema.Data.Database.Identity;
using Gadema.Data.Database.Tasks;

namespace Gadema.Data.Database;

public static class DbInjection
{
    public static IServiceCollection AddGademaData(this IServiceCollection services, IConfiguration configuration)
    {
        // The implementation detail (Sqlite) is hidden inside this method
        services.AddDbContext<CoreDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<WritingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<TaskDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}