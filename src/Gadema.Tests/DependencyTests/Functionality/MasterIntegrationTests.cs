using System.Reflection;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Access;
using Gadema.Tests.Factory;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Functionality;

public class MasterIntegrationTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public MasterIntegrationTests(ApiWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact(Timeout = 30000)] 
    public async Task AutoSeed_AllModels_ShouldSucceedInFKOrder()
    {
        var assembly = typeof(ModelDependencyAttribute).Assembly;
        var result = DependencyResolver.ResolveDependencies(assembly);
        var sortedTypes = result.SortedTypes;
        DbSeeder.ResetSeedingCache();
        Assert.NotNull(sortedTypes);
        Assert.NotEmpty(sortedTypes);

        var results = new List<(Type Type, bool Success, string? Error)>();

        foreach (var type in sortedTypes)
        {
            try
            {
                var autoSeedMethod = typeof(DbSeeder).GetMethod(nameof(DbSeeder.AutoSeed), BindingFlags.Public | BindingFlags.Static)
                    ?.MakeGenericMethod(type);

                if (autoSeedMethod == null)
                {
                    results.Add((type, false, "AutoSeed method not found"));
                    continue;
                }
                
                autoSeedMethod.Invoke(null, new object[] { _scope, (Action<object>?)null });
                results.Add((type, true, null));
            }
             catch (SqliteException ex)
            {
                var inner = ex.InnerException ?? ex;
                results.Add((type, false, inner.Message));
            }
             catch (DbUpdateException ex)
            {
                var inner = ex.InnerException ?? ex;
                results.Add((type, false, inner.Message));
            }
             catch (TargetInvocationException ex)
            {
                Exception e = ex;

                while(e.InnerException != null)
                {
                    e = e.InnerException;
                }
                
                results.Add((type, false, e.Message));
            }
        }

        var failures = results.Where(r => !r.Success).ToList();
        var succeeded = results.Where(r => r.Success).ToList();
        string successfullSeed = $"\r\n Succeeded to seed: \r\n {string.Join($",\r\n ", succeeded.Select(f => $"{f.Type.Name}"))}";
        string failedSeed = $" Failed to seed: \r\n {string.Join($",\r\n ", failures.Select(f => $"{f.Type.Name}: {f.Error} "))}";
        Assert.True(failures.Count == 0, $"{successfullSeed} \r\n \r\n {failedSeed}");
    }
}
