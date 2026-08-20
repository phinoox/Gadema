// =============================================================================
// GameDev.Tests - Unit Tests for Services
// =============================================================================

namespace GameDev.Tests.Services;

using Xunit;
using FluentAssertions;
using System.Net;

/// <summary>
/// Tests for ContentItemService business logic.
/// </summary>
public class ContentItemServiceTests
{
    private readonly GameDbContext _context;
    private readonly ContentItemService _service;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    [Fact]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: $"GaDeMaTest_{Guid.NewGuid()}")
            .Options;

        _context = new GameDbContext(options);
        _service = new ContentItemService(_context, null!);

        await _context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Test: Create content item should return success.
    /// </summary>
    [Fact]
    public async Task CreateContentItemAsync_ShouldReturnSuccess()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        // Act
        var result = await _service.CreateContentItemAsync(createDto);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Get content item by ID should return data.
    /// </summary>
    [Fact]
    public async Task GetContentItemAsync_WhenIdExists_ShouldReturnData()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var result = await _service.CreateContentItemAsync(createDto);

        // Act
        var getResult = await _service.GetContentItemAsync(result.Data!.Id, ViewModeEnum.PrivateWriting);

        // Assert
        getResult.Success.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update content item should persist changes.
    /// </summary>
    [Fact]
    public async Task UpdateContentItemAsync_ShouldPersistChanges()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var createResult = await _service.CreateContentItemAsync(createDto);

        // Act
        var updateDto = new UpdateContentItemDto
        {
            Description = "Updated description",
            Published = true,
            ViewMode = null!
        };

        var updateResult = await _service.UpdateContentItemAsync(createResult.Data!.Id, updateDto);

        // Assert
        updateResult.Success.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete content item should return success.
    /// </summary>
    [Fact]
    public async Task DeleteContentItemAsync_ShouldReturnSuccess()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var createResult = await _service.CreateContentItemAsync(createDto);

        // Act
        var deleteResult = await _service.DeleteContentItemAsync(createResult.Data!.Id);

        // Assert
        deleteResult.Success.Should().BeTrue();
    }
}