// =============================================================================
// Gadema.Tests - Unit Tests for ContentItemService (No Factory)
// =============================================================================

namespace Gadema.Tests.Services;

using Xunit;
using FluentAssertions;
using System.Net;
using Gadema.Data;
using Gadema.Api.Services;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Enums;

/// <summary>
/// Unit tests for ContentItemService business logic.
/// Uses in-memory database with direct service instantiation.
/// </summary>
public class ContentItemServiceUnitTest
{
    private readonly IContentService _service;
    private GameDbContext _context;

    public ContentItemServiceUnitTest()
    {
        // Setup in-memory database context
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        _context = new GameDbContext(options);  // ✅ Fixed: Create instance with correct type
        _service = new ContentItemService(_context, null!);
    }

    /// <summary>
    /// Test: Create content item should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateContentItemAsync_ShouldReturnSuccessful()
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
        result.Successful.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Get content item by ID should return data.
    /// </summary>
    [Fact]
    public async Task GetContentItemAsync_WhenIdExists_ShouldReturnData()
    {
        // Arrange - create first
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

        // Act - get by ID
        var getResult = await _service.GetContentItemAsync(
            createResult.Data!.Id, 
            ViewModeEnum.PrivateWriting);

        // Assert
        getResult.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update content item should persist changes.
    /// </summary>
    [Fact]
    public async Task UpdateContentItemAsync_ShouldPersistChanges()
    {
        // Arrange - create first
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

        // Act - update
        var updateDto = new UpdateContentItemDto
        {
            Description = "Updated description",
            Published = true,
            ViewMode = null!
        };

        var updateResult = await _service.UpdateContentItemAsync(
            createResult.Data!.Id, 
            updateDto);

        // Assert
        updateResult.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete content item should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteContentItemAsync_ShouldReturnSuccessful()
    {
        // Arrange - create first
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

        // Act - delete
        var deleteResult = await _service.DeleteContentItemAsync(
            createResult.Data!.Id);

        // Assert
        deleteResult.Successful.Should().BeTrue();
    }
}