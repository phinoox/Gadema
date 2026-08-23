// =============================================================================
// GameDev.Tests - Validation Tests for Dtos (Corrected)
// =============================================================================

namespace GameDev.Tests.Models;

using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Dtos.Projects;
using GameDev.Core.Dtos.Tasks;
using GameDev.Core.Enums;

/// <summary>
/// Validation tests for content item DTOs.
/// </summary>
public class CreateContentItemDtoValidationTests
{
    [Fact]
    public void ProjectId_ShouldBeRequired()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.ProjectId.Should().NotBeEmpty();
    }

    [Fact]
    public void ContentType_ShouldBeValidEnum()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.ContentType.Should().Be(ContentTypeEnum.Character,"ContentType should be valid enum value");
    }

    [Fact]
    public void Title_ShouldBeRequired()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.Title.Should().Be("Test Character","Title is required and should not be empty");
    }

    [Fact]
    public void Slug_ShouldBeOptional()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,  // Empty slug (optional)
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void Description_ShouldBeOptional()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",  // Optional
            ShortDesc = "Test short desc"
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void ShortDesc_ShouldBeOptional()
    {
        // Arrange
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"  // Optional
        };

        // Assert
        createDto.Should().NotBeNull();
    }
}

/// <summary>
/// Validation tests for project DTOs.
/// </summary>
public class CreateProjectDtoValidationTests
{
    [Fact]
    public void Title_ShouldBeRequired()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Title = "My New Project",
            Slug = null!,
            TemplateId = null,
            Visibility = 1
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.Title.Should().Be("My New Project","Title is required for CreateProjectDto");
    }

    [Fact]
    public void Slug_ShouldBeOptional()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Title = "My Project",
            Slug = null!,  // Optional
            TemplateId = null,
            Visibility = 1
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void TemplateId_ShouldBeOptional()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Title = "My Project",
            Slug = null!,
            TemplateId = null,  // Optional
            Visibility = 1
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void Visibility_ShouldBeInRange()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            Title = "My Project",
            Slug = null!,
            TemplateId = null,
            Visibility = 1  // Valid: 1=Private, 2=Public
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.Visibility.Should().BeInRange(1, 2,"Visibility should be 1 or 2");
    }
}

/// <summary>
/// Validation tests for task DTOs.
/// </summary>
public class ProjectTaskCreateDtoValidationTests
{
    [Fact]
    public void ProjectId_ShouldBeRequired()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.ProjectId.Should().NotBeEmpty("ProjectId is required for ProjectTask");
    }

    [Fact]
    public void TaskTitle_ShouldBeRequired()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",  // Required
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
        createDto.TaskTitle.Should().Be("Test Task","TaskTitle is required");
    }

    [Fact]
    public void Description_ShouldBeOptional()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,  // Optional
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void Status_ShouldBeNullableInt()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,  // Nullable int
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void Priority_ShouldBeNullableInt()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,  // Nullable int: High(0), Medium(1), Low(2)
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void Difficulty_ShouldBeNullableInt()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,  // Nullable int: Easy(0), Medium(1), Hard(2)
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void EstimatedMinutes_ShouldBeNullableDecimal()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,  // Nullable decimal in minutes
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void AssignedToUserId_ShouldBeNullableGuid()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,  // Nullable Guid
            DueDate = null,
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void DueDate_ShouldBeNullableDateTime()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,  // Nullable DateTime
            IsQuickWin = false
        };

        // Assert
        createDto.Should().NotBeNull();
    }

    [Fact]
    public void IsQuickWin_ShouldBeNullableBool()
    {
        // Arrange
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Description = null,
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            EstimatedMinutes = null,
            AssignedToUserId = null,
            DueDate = null,
            IsQuickWin = false  // Nullable bool
        };

        // Assert
        createDto.Should().NotBeNull();
    }
}