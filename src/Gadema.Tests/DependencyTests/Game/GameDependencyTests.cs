using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Game.Attributes;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Game;


public class GameDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public GameDependencyTests(ApiWebApplicationFactory factory)
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
        var attributeSetType = typeof(AttributeSet);
        // This won't fail a build, but will print the beautiful tree to your console!
        DependencyResolver.PrintHierarchy(attributeSetType.Assembly,"Gadema.Core.Models.Game");
    }

}