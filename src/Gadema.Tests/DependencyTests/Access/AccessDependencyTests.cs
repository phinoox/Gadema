using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Access;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Access;

public class AccessDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public AccessDependencyTests(ApiWebApplicationFactory factory)
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
        var userType = typeof(User);
        DependencyResolver.PrintHierarchy(userType.Assembly, "Gadema.Core.Models.Access");
    }
}
