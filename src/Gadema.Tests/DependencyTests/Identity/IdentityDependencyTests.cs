using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Identity;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Identity;

public class IdentityDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public IdentityDependencyTests(ApiWebApplicationFactory factory)
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
        var identityType = typeof(IdentityDefinition);
        DependencyResolver.PrintHierarchy(identityType.Assembly, "Gadema.Core.Models.Identity");
    }
}
