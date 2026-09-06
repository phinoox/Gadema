// =============================================================================
// Gadema.Tests - Unit Tests for MetaInfoService (No Factory)
// =============================================================================

namespace Gadema.Tests.Services;

using Xunit;
using FluentAssertions;
using System.Net;
using Gadema.Data.Database;
using Gadema.Api.Services;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Enums;

/// <summary>
/// Unit tests for MetaInfoService business logic.
/// Uses in-memory database with direct service instantiation.
/// </summary>
public class MetaInfoServiceUnitTest
{
    private readonly IContentService _service;
    private GameDbContext _context;

    public MetaInfoServiceUnitTest()
    {
        // Setup in-memory database context
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        _context = new GameDbContext(options);  // ✅ Fixed: Create instance with correct type
        _service = new MetaInfoService(_context, null!);
    }

    /// <summary>
    /// Test: Create content item should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateMetaInfoAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var createDto = new CreateMetaInfoDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        // Act
        var result = await _service.CreateMetaInfoAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Get content item by ID should return data.
    /// </summary>
    [Fact]
    public async Task GetMetaInfoAsync_WhenIdExists_ShouldReturnData()
    {
        // Arrange - create first
        var createDto = new CreateMetaInfoDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var createResult = await _service.CreateMetaInfoAsync(createDto);

        // Act - get by ID
        var getResult = await _service.GetMetaInfoAsync(
            createResult.Data!.Id.Value, 
            ViewModeEnum.PrivateWriting);

        // Assert
        getResult.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update content item should persist changes.
    /// </summary>
    [Fact]
    public async Task UpdateMetaInfoAsync_ShouldPersistChanges()
    {
        // Arrange - create first
        var createDto = new CreateMetaInfoDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var createResult = await _service.CreateMetaInfoAsync(createDto);

        // Act - update
        var updateDto = new UpdateMetaInfoDto
        {
            Description = "Updated description",
            Published = true,
            ViewMode = null!
        };

        var updateResult = await _service.UpdateMetaInfoAsync(
            createResult.Data!.Id.Value, 
            updateDto);

        // Assert
        updateResult.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete content item should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteMetaInfoAsync_ShouldReturnSuccessful()
    {
        // Arrange - create first
        var createDto = new CreateMetaInfoDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Geralt of Rivia",
            Slug = null!,
            Description = "Main protagonist character",
            ShortDesc = "Witcher"
        };

        var createResult = await _service.CreateMetaInfoAsync(createDto);

        // Act - delete
        var deleteResult = await _service.DeleteMetaInfoAsync(
            createResult.Data!.Id.Value);

        // Assert
        deleteResult.Successful.Should().BeTrue();
    }
}