using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Enums;
using Gadema.Core.Models.Base.Projects;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Gadema.Tests.Factory;
using Gadema.Core.Models.Access;
using Gadema.Data.Database;

namespace Gadema.Tests.Unit.Services;

public class ProjectServiceTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly ProjectService _projectService;

    public ProjectServiceTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _projectService = factory.GetScopedService<ProjectService>();
        _factory.ResetDb();
    }

    [Fact]
    public async Task CreateAsync_ShouldSucceed_WhenValidDataIsProvided()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        
        // Setup UserContext for the service to see a logged-in user
        var userContext = TestUserContextHelper.CreateTestContext(_factory);
        userContext.CurrentUser = owner;

        var createDto = new ProjectCreateDto
        {
            Description = "New test project",
            EnableUserRegistration = true,
            AllowManualInvites = false,
            PrimaryFormat = PrimaryFormatEnum.Book,
            Genre = "Fantasy",
            Theme = "Magic",
            Tone = ToneEnum.Dark,
            Audience = AudienceEnum.Adult,
            MetaInfo = new ProjectMetaInfoCreateData
            {
                Title = "the Punisher",
            }
        };

        // Act
        var result = await _projectService.CreateAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
        result.Data!.ProjectId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnProject_WhenExists()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        var project = DbSeeder.Create<Project>(p => p.User = owner);
        DbSeeder.Seed(scope, project);
        project = DbSeeder.AutoSeed<Project>(scope);

        // Act
        var result = await _projectService.GetAsync(project.Id, project.Id);

        // Assert
        result.Successful.Should().BeTrue();
        result.Data!.Id.Should().Be(project.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldApplyChanges_WhenFieldsAreProvided()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        var project = DbSeeder.Create<Project>(p => p.User = owner);
        DbSeeder.Seed(scope, project);

        var updateDto = new ProjectUpdateDto
        {
            Description = "Updated description",
            Genre = "New Genre"
        };

        // Act
        var result = await _projectService.UpdateAsync(project.Id, project.Id, updateDto);

        // Assert
        result.Successful.Should().BeTrue();
        
        var db = _factory.GetScopedService<GameDbContext>();
        var updatedProject = await db.Projects
            .Include(p => p.ProjectMetaInfo)
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        updatedProject!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProject_WhenAuthorized()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        var project = DbSeeder.Create<Project>(p => p.User = owner);
        DbSeeder.Seed(scope, project);

        // Act
        var result = await _projectService.DeleteAsync(project.Id, project.Id);

        // Assert
        result.Successful.Should().BeTrue();
        
        var db = _factory.GetScopedService<GameDbContext>();
        var deletedProject = await db.Projects.FindAsync(project.Id);
        deletedProject.Should().BeNull();
    }
}
