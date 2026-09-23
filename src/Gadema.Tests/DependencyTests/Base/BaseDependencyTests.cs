using System.ComponentModel;
using FluentAssertions;
using Gadema.Core.DependencyTracking;
using Gadema.Core.Models.Base;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Utils;
using Gadema.Tests.Factory;
using Gadema.Tests.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gadema.Tests.DependencyTests.Base;

public class BaseDependencyTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    private readonly string _domainName = "Gadema.Core.Models.Base";

    public BaseDependencyTests(ApiWebApplicationFactory factory)
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
        var projectType = typeof(Project);
        DependencyResolver.PrintHierarchy(projectType.Assembly, _domainName);
    }

    [Fact]
    public void ProjectMetaInfo_WithoutProjectId_ShouldSucceed()
    {
        // Arrange: Create a MetaInfo that points to a non-existent Project ID
        var metaInfo = DbSeeder.Create<ProjectMetaInfo>();

        // Act & Assert: Call Seed synchronously as the method is not Task-returning
        metaInfo.Should().NotBeNull();
    }

    [Fact]
    public void Project_WhenAutoSeeded_ShouldHaveLinkedMetaInfo()
    {
        // Act
        var project = DbSeeder.AutoSeed<Project>(_scope);

        // Assert
        project.Should().NotBeNull();
        project.ProjectMetaInfo.Should().NotBeNull("because AutoSeed should resolve mandatory 1:1 dependencies");
        project.ProjectMetaInfo.ProjectId.Should().Be(project.Id, "because the FK must match the parent ID");
    }

    [Fact]
    public void Project_WhenAutoSeeded_ShouldHaveLinkedUser()
    {
        // Act
        var project = DbSeeder.AutoSeed<Project>(_scope);

        // Assert
        project.Should().NotBeNull();
        project.User.Should().NotBeNull("because AutoSeed should resolve mandatory 1:1 dependencies");
       
    }

    [Fact]
    public void Project_WhenCustomized_ShouldMaintainIntegrity()
    {
        // Act: Seed a project and manually add a tag during customization
        var project = DbSeeder.AutoSeed<Project>(_scope,_domainName, p => {
            p.Description = "Seeded";
           // p.ProjectMetaInfo.TagIds.Add(BaseTag.Lore.ToGuid());
            // We can use the existing instance to build related data
            // (Note: In a real test, you'd ensure your customization logic 
            // respects the DB constraints)
        });

        project.Should().NotBeNull();
    }

    

    [Fact]
    public void ProjectMetaInfo_WithMismatchedProjectId_ShouldThrowException()
    {
        // Arrange: Create a MetaInfo that points to a non-existent Project ID
        var mismatchedMeta = DbSeeder.Create<ProjectMetaInfo>(m => 
        {
            m.ProjectId = Guid.NewGuid(); // A random ID that hasn't been seeded
        });

        // Act & Assert: Call Seed synchronously as the method is not Task-returning
        Assert.Throws<DbUpdateException>(() => DbSeeder.Seed(_scope, mismatchedMeta));
    }


    [Fact]
    public void ProjectTag_WithoutValidRelation_ShouldNotViolateConstraints()
    {
        // Since ProjectTag has no direct ProjectId, we test that it can exist 
        // as a standalone entity (a "Root" in its own context)
        var tag = DbSeeder.Create<ProjectTag>(t => {
            t.Name = "Test Tag";
        });

        // Act & Assert: Seeding should succeed because there are no mandatory FKs on ProjectTag itself
        var seededTag = DbSeeder.Seed(_scope, tag);
        seededTag.Id.Should().NotBeEmpty();
    }
    
}
