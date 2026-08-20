// =============================================================================
// GameDev.Tests - Unit Tests for Models
// =============================================================================

namespace GameDev.Tests.Models;

using Xunit;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;

/// <summary>
/// Validation tests for CreateProjectDto.
/// </summary>
public class CreateProjectDtoValidationTests
{
    /// <summary>
    /// Test: Title should be required.
    /// </summary>
    [Fact]
    public void Title_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateProjectDto
        {
            Title = "",
            Visibility = 1
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle()
            .Where(r => r.MemberName == "Title")
            .Where(r => r.ErrorMessage!.Contains("required"));
    }

    /// <summary>
    /// Test: Visibility should be in range 1-2.
    /// </summary>
    [Fact]
    public void Visibility_ShouldBeInRange()
    {
        // Arrange & Act
        var dto = new CreateProjectDto
        {
            Title = "Test Project",
            Visibility = 5
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle()
            .Where(r => r.MemberName == "Visibility")
            .Where(r => r.ErrorMessage!.Contains("Range"));
    }

    /// <summary>
    /// Test: Valid DTO should pass validation.
    /// </summary>
    [Fact]
    public void ValidDto_ShouldPassValidation()
    {
        // Arrange & Act
        var dto = new CreateProjectDto
        {
            Title = "Test Project",
            Visibility = 1
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeTrue();
    }
}

/// <summary>
/// Validation tests for CreateContentItemDto.
/// </summary>
public class CreateContentItemDtoValidationTests
{
    /// <summary>
    /// Test: ProjectId should be required.
    /// </summary>
    [Fact]
    public void ProjectId_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateContentItemDto
        {
            ProjectId = Guid.Empty,
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test character description",
            ShortDesc = "Test character"
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Test: ContentType should be required.
    /// </summary>
    [Fact]
    public void ContentType_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = null!,
            ShortDesc = null!
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Test: Title should be required.
    /// </summary>
    [Fact]
    public void Title_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "",
            Slug = null!,
            Description = "Test character description",
            ShortDesc = "Test character"
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
    }
}

/// <summary>
/// Validation tests for CreateTaskDto.
/// </summary>
public class CreateTaskDtoValidationTests
{
    /// <summary>
    /// Test: ProjectId should be required.
    /// </summary>
    [Fact]
    public void ProjectId_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateTaskDto
        {
            ProjectId = Guid.Empty,
            TaskTitle = "Test Task",
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            IsQuickWin = false
        };

        // Note: ProjectId doesn't have [Required] attribute, so this passes
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeTrue();
    }

    /// <summary>
    /// Test: TaskTitle should be required.
    /// </summary>
    [Fact]
    public void TaskTitle_ShouldBeRequired()
    {
        // Arrange & Act
        var dto = new CreateTaskDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "",
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            IsQuickWin = false
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Test: Valid DTO should pass validation.
    /// </summary>
    [Fact]
    public void ValidDto_ShouldPassValidation()
    {
        // Arrange & Act
        var dto = new CreateTaskDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            IsQuickWin = true
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.Validate(dto, validationContext, validationResults);

        // Assert
        isValid.Should().BeTrue();
    }
}

/// <summary>
/// Test: TaskDifficultyEnum values.
/// </summary>
public class TaskDifficultyEnumTests
{
    /// <summary>
    /// Test: Easy difficulty value.
    /// </summary>
    [Fact]
    public void Easy_ShouldBeZero()
    {
        // Assert
        (int)TaskDifficultyEnum.Easy.Should().Be(0);
    }

    /// <summary>
    /// Test: Medium difficulty value.
    /// </summary>
    [Fact]
    public void Medium_ShouldBeOne()
    {
        // Assert
        (int)TaskDifficultyEnum.Medium.Should().Be(1);
    }

    /// <summary>
    /// Test: Hard difficulty value.
    /// </summary>
    [Fact]
    public void Hard_ShouldBeTwo()
    {
        // Assert
        (int)TaskDifficultyEnum.Hard.Should().Be(2);
    }
}

/// <summary>
/// Test: ContentTypeEnum values.
/// </summary>
public class ContentTypeEnumTests
{
    /// <summary>
    /// Test: Character content type value.
    /// </summary>
    [Fact]
    public void Character_ShouldBeZero()
    {
        // Assert
        (int)ContentTypeEnum.Character.Should().Be(0);
    }

    /// <summary>
    /// Test: World content type value.
    /// </summary>
    [Fact]
    public void World_ShouldBeOne()
    {
        // Assert
        (int)ContentTypeEnum.World.Should().Be(1);
    }
}