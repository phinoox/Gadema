using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Writing;


public class WritingDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public WritingDependencyTests(ApiWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }

    [Fact]
    public void Visual_Dependency_Check()
    {
        var storyType = typeof(Story);
        // This won't fail a build, but will print the beautiful tree to your console!
        DependencyResolver.PrintHierarchy(storyType.Assembly,"Gadema.Core.Models.Writing");
    }

}