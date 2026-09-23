using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Access;
using Gadema.Tests.DependencyTests.Functionality.Models;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Functionality;


public class ResolverTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public ResolverTests(ApiWebApplicationFactory factory)
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
        var result = DependencyResolver.ResolveDependencies(assembly,"Gadema.Tests.DependencyTests.Functionality.Models");
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
        var result = DependencyResolver.ResolveDependencies(assembly,"Gadema.Tests.DependencyTests.Functionality.Models");
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


}