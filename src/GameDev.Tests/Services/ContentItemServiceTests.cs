// =============================================================================
// GameDev.Tests - Unit Tests for Services
// =============================================================================

namespace GameDev.Tests.Services;

using Xunit;
using FluentAssertions;
using System.Net;
using GameDev.Data;
using GameDev.Api.Services;
using Microsoft.EntityFrameworkCore;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Tests for ContentItemService business logic.
/// </summary>
public class ContentItemServiceTests
{
    private readonly IContentService _service; // ✅ Use interface!

    public ContentItemServiceTests(ApiWebApplicationFactory factory)
    {
        _service = factory.Services.GetRequiredService<IContentService>(); // ✅ DI!
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
        getResult.Successful.Should().BeTrue();
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
        updateResult.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete content item should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteContentItemAsync_ShouldReturnSuccessful()
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
        deleteResult.Successful.Should().BeTrue();
    }
}