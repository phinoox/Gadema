using System;
using System.Linq;
using System.Reflection;
using Gadema.Core.DependencyResolver;
using Gadema.Core.Models;
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
        Assert.Equal(4,result.SortedTypes.Count);
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
        Assert.Equal(2,result.CyclicTypes.Count);
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
    public void AutoSeed_AllModels_ShouldSucceedInFKOrder()
    {
        var assembly = typeof(ModelDependencyAttribute).Assembly;
        var result = DependencyResolver.ResolveDependencies(assembly);
        var sortedTypes = result.SortedTypes;

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
            catch (TargetInvocationException ex)
            {
                var inner = ex.InnerException ?? ex;
                results.Add((type, false, inner.Message));
            }
        }

        var failures = results.Where(r => !r.Success).ToList();
        Assert.True(failures.Count == 0, $"Failed to seed: {string.Join(", ", failures.Select(f => $"{f.Type.Name}: {f.Error}"))}");
    }
}