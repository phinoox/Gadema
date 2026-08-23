// =============================================================================
// GameDev.Tests - Unit Tests for Service Logic
// =============================================================================

namespace GameDev.Tests.Services;

using Xunit;
using FluentAssertions;
using Moq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using GameDev.Api.Services;
using GameDev.Core.Dtos.Authentication;
using GameDev.Core.Dtos.Projects;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Enums;
using GameDev.Core.Dtos.Tasks;
using GameDev.Core.Dtos.Export;
using GameDev.Core.Dtos.Reviews;

/// <summary>
/// Integration tests for authentication service.
/// </summary>
public class ApiAuthServiceIntegrationTests
{
    private readonly Mock<ILogger<ApiAuthService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ApiAuthServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ApiAuthService>>();
    }

    /// <summary>
    /// Test: Google callback should return Successful.
    /// </summary>
    [Fact]
    public async Task GoogleCallbackAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var authService = new ApiAuthService(null!, null!, _mockLogger.Object);

        // Act
        var result = await authService.GoogleCallbackAsync("test_code");

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Disable 2FA should return Successful.
    /// </summary>
    [Fact]
    public async Task Disable2FAAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var authService = new ApiAuthService(null!, null!, _mockLogger.Object);

        // Act
        var result = await authService.Disable2FAAsync(new Disable2FADto
        {
            TwoFactorToken = "123456"
        });

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for project service.
/// </summary>
public class ProjectServiceIntegrationTests
{
    private readonly Mock<ILogger<ProjectService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ProjectServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ProjectService>>();
    }

    /// <summary>
    /// Test: Create project should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateProjectAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ProjectService(null!, _mockLogger.Object);
        var createDto = new CreateProjectDto
        {
            Title = "Test Project",
            Visibility = 1
        };

        // Act
        var result = await service.CreateProjectAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update project should return Successful.
    /// </summary>
    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ProjectService(null!, _mockLogger.Object);

        // Act
        var result = await service.UpdateProjectAsync(Guid.NewGuid(), new UpdateProjectDto());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for content item service.
/// </summary>
public class ContentItemServiceIntegrationTests
{
    private readonly Mock<ILogger<ContentItemService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ContentItemServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ContentItemService>>();
    }

    /// <summary>
    /// Test: Create content item should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateContentItemAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ContentItemService(null!, _mockLogger.Object);
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Act
        var result = await service.CreateContentItemAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Get content item should return Successful.
    /// </summary>
    [Fact]
    public async Task GetContentItemAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ContentItemService(null!, _mockLogger.Object);

        // Act
        var result = await service.GetContentItemAsync(Guid.NewGuid(), ViewModeEnum.PrivateWriting);

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete content item should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteContentItemAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ContentItemService(null!, _mockLogger.Object);

        // Act
        var result = await service.DeleteContentItemAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for task service.
/// </summary>
public class TaskServiceIntegrationTests
{
    private readonly Mock<ILogger<ProjectTaskService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public TaskServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ProjectTaskService>>();
    }

    /// <summary>
    /// Test: Create task should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateTaskAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ProjectTaskService(null!, _mockLogger.Object);
        var createDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            IsQuickWin = true
        };

        // Act
        var result = await service.CreateTaskAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update task should return Successful.
    /// </summary>
    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ProjectTaskService(null!, _mockLogger.Object);

        // Act
        var result = await service.UpdateTaskAsync(Guid.NewGuid(), new ProjectTaskUpdateDto());

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Delete task should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ProjectTaskService(null!, _mockLogger.Object);

        // Act
        var result = await service.DeleteTaskAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for export service.
/// </summary>
public class ExportServiceIntegrationTests
{
    private readonly Mock<ILogger<ExportService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ExportServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ExportService>>();
    }

    /// <summary>
    /// Test: Export to JSON should return OK.
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_ShouldReturnOk()
    {
        // Arrange
        var service = new ExportService(_mockLogger.Object);

        // Act
        var result = await service.ExportToJsonAsync(Guid.NewGuid(), new ExportJsonDto());

        // Assert
        result.Should().NotBeNull();
    }

    /// <summary>
    /// Test: Export to CSV should return File content.
    /// </summary>
    [Fact]
    public async Task ExportToCsvAsync_ShouldReturnFile()
    {
        // Arrange
        var service = new ExportService(_mockLogger.Object);

        // Act
        var result = await service.ExportToCsvAsync(Guid.NewGuid(), new ExportCsvDto());

        // Assert
        result.Should().NotBeNull();
    }
}

/// <summary>
/// Integration tests for review status service.
/// </summary>
public class ReviewStatusServiceIntegrationTests
{
    private readonly Mock<ILogger<ReviewStatusService>> _mockLogger;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ReviewStatusServiceIntegrationTests()
    {
        _mockLogger = new Mock<ILogger<ReviewStatusService>>();
    }

    /// <summary>
    /// Test: Get review status should return Successful.
    /// </summary>
    [Fact]
    public async Task GetReviewStatusAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ReviewStatusService(null!, _mockLogger.Object);

        // Act
        var result = await service.GetReviewStatusAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Approve content should return Successful.
    /// </summary>
    [Fact]
    public async Task ApproveContentAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var service = new ReviewStatusService(null!, _mockLogger.Object);

        // Act
        var result = await service.ApproveContentAsync(Guid.NewGuid(), new ApproveContentDto
        {
            Status = 1,
            ReviewComments = "Approved"
        });

        // Assert
        result.Successful.Should().BeTrue();
    }
}