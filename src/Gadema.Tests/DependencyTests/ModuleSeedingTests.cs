using System.Reflection;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base;
using Gadema.Core.Models.Game;
using Gadema.Core.Models.Identity;
using Gadema.Core.Models.Tasks;
using Gadema.Core.Models.Writing;
using Gadema.Tests.Factory;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests;

public class ModuleSeedingTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public ModuleSeedingTests(ApiWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Theory]
    [InlineData("Gadema.Core.Models.Access")]
    [InlineData("Gadema.Core.Models.Base")]
    [InlineData("Gadema.Core.Models.Identity")]
    [InlineData("Gadema.Core.Models.Tasks")]
    [InlineData("Gadema.Core.Models.Writing")]
    [InlineData("Gadema.Core.Models.Game")]
    public async Task AutoSeed_ModuleNamespace_ShouldSucceedInFKOrder(string domainNamespace)
    {
        DbSeeder.ResetSeedingCache();

        // We use the assembly of a known type to get the assembly reference
        var assembly = typeof(ModelDependencyAttribute).Assembly;
        
        // Get sorted types for this specific namespace
        var (sortedTypes, _) = DependencyResolver.ResolveDependencies(assembly, domainNamespace);
        
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
                
                autoSeedMethod.Invoke(null, new object[] { _scope, domainNamespace, (Action<object>?)null });
                results.Add((type, true, null));
            }
            catch (Exception ex)
            {
                Exception actualEx = ex;
                while (actualEx.InnerException != null) actualEx = actualEx.InnerException;
                results.Add((type, false, actualEx.Message));
            }
        }

        var failures = results.Where(r => !r.Success).ToList();
        var succeeded = results.Where(r => r.Success).ToList; // Fixed typo in previous logic: succeeded.Select -> succeeded
        // Wait, I'll just write the assertion simply for brevity here.
        
        Assert.True(failures.Count == 0, $"Namespace {domainNamespace} failed to seed: \r\n {string.Join($",\r\n ", failures.Select(f => $"{f.Type.Name}: {f.Error} "))}");
    }
}
