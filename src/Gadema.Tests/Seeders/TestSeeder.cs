using System;
using System.Linq;
using System.Reflection;
using Gadema.Core.DependencyResolver;
using Gadema.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.Seeders;

// ─── Cycle test types ─────────────────────────────────────────────────────
[ModelDependency(typeof(CycleTypeB))]
public class CycleTypeA { }

[ModelDependency(typeof(CycleTypeA))]
public class CycleTypeB { }

// ─── Topological order test types ─────────────────────────────────────────
// Linear chain: TopoTypeD → TopoTypeC → TopoTypeB → TopoTypeA
[ModelDependency(typeof(TopoTypeB))]
public class TopoTypeA { }

[ModelDependency(typeof(TopoTypeC))]
public class TopoTypeB { }

[ModelDependency(typeof(TopoTypeD))]
public class TopoTypeC { }

[ModelDependency(typeof(RootMarker))]
public class TopoTypeD { } // root — no dependencies

public class DependencyGraphTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public DependencyGraphTests(ApiWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }



    [Fact]
    public void ResolveDependencies_ShouldReturnCorrectOrderForIsolatedGraph()
    {
        // Test with only the TopoType* types — no Gadema.Core models involved
        var assembly = typeof(TopoTypeA).Assembly;
        var result = DependencyResolver.ResolveDependencies(assembly);
        Assert.NotNull(result.SortedTypes);
        Assert.NotEmpty(result.SortedTypes);
        Assert.Equal(4, result.SortedTypes.Count);
        // TopoTypeD has no dependencies, so it should be first
        var topoTypeDIndex = result.SortedTypes.IndexOf(typeof(TopoTypeD));
        var topoTypeCIndex = result.SortedTypes.IndexOf(typeof(TopoTypeC));
        var topoTypeBIndex = result.SortedTypes.IndexOf(typeof(TopoTypeB));
        var topoTypeAIndex = result.SortedTypes.IndexOf(typeof(TopoTypeA));
        // Each type must appear AFTER its dependency
        Assert.True(topoTypeDIndex < topoTypeCIndex, "TopoTypeD should come before TopoTypeC");
        Assert.True(topoTypeCIndex < topoTypeBIndex, "TopoTypeC should come before TopoTypeB");
        Assert.True(topoTypeBIndex < topoTypeAIndex, "TopoTypeB should come before TopoTypeA");
    }

    [Fact]
    public void ResolveDependencies_ShouldThrowOnCycle()
    {
        var assembly = typeof(TopoTypeA).Assembly;
        var result = DependencyResolver.ResolveDependencies(assembly);
        Assert.NotNull(result.CyclicTypes);
        Assert.NotEmpty(result.CyclicTypes);
        Assert.Equal(2, result.CyclicTypes.Count);
    }


    [Fact]
    public void ResolveDependencies_ShouldReturnTopologicalOrder()
    {
        var assembly = typeof(User).Assembly;
        var result = DependencyResolver.ResolveDependencies(assembly);

        Assert.NotNull(result.SortedTypes);
        Assert.NotEmpty(result.SortedTypes);
        Assert.Contains(typeof(User), result.SortedTypes);
    }

    [Fact]
    public void AllModels_ShouldHaveModelDependencyAttribute()
    {
        var assembly = typeof(ModelDependencyAttribute).Assembly;
        var modelsAssembly = typeof(Gadema.Core.Models.User).Assembly;

        // Get all classes in Gadema.Core.Models namespace
        var modelTypes = modelsAssembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace?.StartsWith("Gadema.Core.Models") == true)
            .ToList();

        // Get all types with ModelDependency attribute
        var attributedTypes = new HashSet<Type>();
        foreach (var type in assembly.GetExportedTypes())
        {
            var attrs = type.GetCustomAttributes(typeof(ModelDependencyAttribute), false);
            if (attrs.Length > 0)
                attributedTypes.Add(type);
        }

        // Every model should have the attribute (except RootMarker itself)
        var missing = modelTypes.Where(t => !attributedTypes.Contains(t) && t.Name != "RootMarker").ToList();

        Assert.True(missing.Count == 0, $"Missing [ModelDependency] attribute on: {string.Join(", ", missing.Select(t => t.Name))}");
    }

    [Fact]
    public void AutoSeed_AllModels_ShouldSucceedInFKOrder()
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