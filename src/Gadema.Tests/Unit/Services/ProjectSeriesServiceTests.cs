using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Gadema.Tests.Factory;
using Gadema.Tests.Helpers;
using Gadema.Tests.Seeders;
using Gadema.Core.Interfaces;
using Gadema.Data.Database.Game;
using Gadema.Data.Database.Core;

namespace Gadema.Tests.Unit.Services;

public class ProjectSeriesServiceTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly ProjectSeriesService _service;
    private readonly IUserContext _userContext;

    public ProjectSeriesServiceTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _service = factory.GetScopedService<ProjectSeriesService>();
        _userContext = TestUserContextHelper.CreateTestContext(_factory);
        _factory.ResetDb();
    }

    [Fact]
    public async Task GetSeriesAsync_ShouldReturnAllSeries()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var meta1 = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Series 1"; m.Slug = "series-1"; m.Description = "Desc 1"; });
        DbSeeder.Seed(scope, meta1);
        var series1 = DbSeeder.Create<ProjectSeries>(s => { s.Id = Guid.NewGuid(); s.ProjectSeriesMetaInfo = meta1; });
        DbSeeder.Seed(scope, series1);

        var meta2 = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Series 2"; m.Slug = "series-2"; m.Description = "Desc 2"; });
        DbSeeder.Seed(scope, meta2);
        var series2 = DbSeeder.Create<ProjectSeries>(s => { s.Id = Guid.NewGuid(); s.ProjectSeriesMetaInfo = meta2; });
        DbSeeder.Seed(scope, series2);

        // Act
        var result = await _service.GetSeriesAsync();

        // Assert
        result.Successful.Should().BeTrue();
        result.Data!.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetSeriesAsync_WithId_ShouldReturnCorrectSeries()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var id = Guid.NewGuid();
        var meta = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Target"; m.Slug = "target"; m.Description = "desc"; });
        DbSeeder.Seed(scope, meta);
        var series = DbSeeder.Create<ProjectSeries>(s => { s.Id = id; s.ProjectSeriesMetaInfo = meta; });
        DbSeeder.Seed(scope, series);

        // Act
        var result = await _service.GetSeriesAsync(id);

        // Assert
        result.Successful.Should().BeTrue();
        result.Data!.Id.Should().Be(id);
        result.Data.Title.Should().Be("Target");
    }

    [Fact]
    public async Task GetSeriesAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _service.GetSeriesAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateSeriesAsync_ShouldCreateSeriesAndMeta()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var createDto = new ProjectSeriesCreateDto
        {
            MetaInfo = new ProjectSeriesMetaInfoCreateData
            {
                Title = "New Series",
                Slug = "new-series"
            },
            Description = "A new series description"
        };

        // Act
        var result = await _service.CreateSeriesAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
        result.Data!.EntityId.Should().NotBeEmpty();

        var db = _factory.GetScopedService<CoreDbContext>();
        var series = await db.ProjectSeries.Include(s => s.ProjectSeriesMetaInfo).FirstOrDefaultAsync();
        series.Should().NotBeNull();
        series!.ProjectSeriesMetaInfo.Title.Should().Be("New Series");
    }

    [Fact]
    public async Task UpdateSeriesAsync_ShouldUpdateDescription()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var id = Guid.NewGuid();
        var meta = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Old Title"; m.Slug = "old-slug"; m.Description = "Old Desc"; });
        DbSeeder.Seed(scope, meta);
        var series = DbSeeder.Create<ProjectSeries>(s => { s.Id = id; s.ProjectSeriesMetaInfo = meta; });
        DbSeeder.Seed(scope, series);

        var updateDto = new ProjectSeriesUpdateDto
        {
            Description = "New Description",
            MetaInfo = null // Don't update identity for this test
        };

        // Act
        var result = await _service.UpdateSeriesAsync(id, updateDto);

        // Assert
        result.Successful.Should().BeTrue();
        var db = _factory.GetScopedService<CoreDbContext>();
        var updated = await db.ProjectSeries.Include(s => s.ProjectSeriesMetaInfo).FirstOrDefaultAsync(s => s.Id == id);
        updated!.ProjectSeriesMetaInfo.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task DeleteSeriesAsync_ShouldRemoveSeriesAndMeta()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var id = Guid.NewGuid();
        var meta = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "To Delete"; m.Slug = "to-delete"; });
        DbSeeder.Seed(scope, meta);
        var series = DbSeeder.Create<ProjectSeries>(s => { s.Id = id; s.ProjectSeriesMetaInfo = meta; });
        DbSeeder.Seed(scope, series);

        // Act
        var result = await _service.DeleteSeriesAsync(id);

        // Assert
        result.Successful.Should().BeTrue();
        var db = _factory.GetScopedService<CoreDbContext>();
        var deleted = await db.ProjectSeries.FindAsync(id);
        deleted.Should().BeNull();

        var metaDeleted = await db.Set<ProjectSeriesMetaInfo>().FirstOrDefaultAsync(m => m.Id == meta.Id);
        metaDeleted.Should().BeNull();
    }

    [Fact]
    public async Task GetMatchesAsync_ShouldReturnResults()
    {
        // Arrange
        var scope = _factory.GetScope();
        var owner = DbSeeder.Seed<User>(scope);
        _userContext.CurrentUser = owner;

        var meta1 = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Alpha Series"; m.Slug = "alpha"; });
        DbSeeder.Seed(scope, meta1);
        var s1 = DbSeeder.Create<ProjectSeries>(s => { s.Id = Guid.NewGuid(); s.ProjectSeriesMetaInfo = meta1; });
        DbSeeder.Seed(scope, s1);

        var meta2 = DbSeeder.Create<ProjectSeriesMetaInfo>(m => { m.Title = "Beta Series"; m.Slug = "beta"; });
        DbSeeder.Seed(scope, meta2);
        var s2 = DbSeeder.Create<ProjectSeries>(s => { s.Id = Guid.NewGuid(); s.ProjectSeriesMetaInfo = meta2; });
        DbSeeder.Seed(scope, s2);

        // Act
        var results = await _service.GetMatchesAsync("Alpha", null);

        // Assert
        results.Should().HaveCount(1);
        results.First().DisplayName.Should().Be("Alpha Series");
    }
}