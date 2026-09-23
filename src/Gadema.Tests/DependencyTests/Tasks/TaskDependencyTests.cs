using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Tasks;
using Gadema.Tests.Factory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Tasks;

public class TaskDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public TaskDependencyTests(ApiWebApplicationFactory factory)
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
        var taskType = typeof(ProjectTask);
        DependencyResolver.PrintHierarchy(taskType.Assembly, "Gadema.Core.Models.Tasks");
    }
}
