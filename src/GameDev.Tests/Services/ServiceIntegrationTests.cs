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
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Integration tests for authentication service.
/// </summary>
public class ApiAuthServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IApiAuthService _authService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ApiAuthServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
         _authService = factory.Services.GetRequiredService<IApiAuthService>();
    }

    /// <summary>
    /// Test: Google callback should return Successful.
    /// </summary>
    [Fact]
    public async Task GoogleCallbackAsync_ShouldReturnSuccessful()
    {
        // Arrange

        // Act
        var result = await _authService.GoogleCallbackAsync("test_code");

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

        // Act
        var result = await _authService.Disable2FAAsync(new Disable2FADto
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
public class ProjectServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IProjectService _projectService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ProjectServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
         _projectService = factory.Services.GetRequiredService<IProjectService>();
    }

    /// <summary>
    /// Test: Create project should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateProjectAsync_ShouldReturnSuccessful()
    {
        // Arrange
       
        var createDto = new CreateProjectDto
        {
            Title = "Test Project",
            Visibility = 1
        };

        // Act
        var result = await _projectService.CreateProjectAsync(createDto);

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
       
        // Act
        var result = await _projectService.UpdateProjectAsync(Guid.NewGuid(), new UpdateProjectDto());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for content item service.
/// </summary>
public class ContentItemServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IContentService _contentItemService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ContentItemServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _contentItemService = factory.Services.GetRequiredService<IContentService>();
        
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
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Act
        var result = await _contentItemService.CreateContentItemAsync(createDto);

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
        // Act
        var result = await _contentItemService.GetContentItemAsync(Guid.NewGuid(), ViewModeEnum.PrivateWriting);

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
      

        // Act
        var result = await _contentItemService.DeleteContentItemAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for task service.
/// </summary>
public class TaskServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IProjectTaskService _projectTaskService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public TaskServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _projectTaskService = factory.Services.GetRequiredService<IProjectTaskService>();
        
    }

    /// <summary>
    /// Test: Create task should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateTaskAsync_ShouldReturnSuccessful()
    {
        // Arrange
     
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
        var result = await _projectTaskService.CreateTaskAsync(createDto);

        // Assert
        result.Successful.Should().BeTrue();
    }

    /// <summary>
    /// Test: Update task should return Successful.
    /// </summary>
    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnSuccessful()
    {
     
        // Act
        var result = await _projectTaskService.UpdateTaskAsync(Guid.NewGuid(), new ProjectTaskUpdateDto());

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
      
        // Act
        var result = await _projectTaskService.DeleteTaskAsync(Guid.NewGuid());

        // Assert
        result.Successful.Should().BeTrue();
    }
}

/// <summary>
/// Integration tests for export service.
/// </summary>
public class ExportServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IExportService _exportService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ExportServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _exportService = factory.Services.GetRequiredService<IExportService>();
        
    }

    /// <summary>
    /// Test: Export to JSON should return OK.
    /// </summary>
    [Fact]
    public async Task ExportToJsonAsync_ShouldReturnOk()
    {
        // Arrange
     

        // Act
        var result = await _exportService.ExportToJsonAsync(Guid.NewGuid(), new ExportJsonDto());

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
    

        // Act
        var result = await _exportService.ExportToCsvAsync(Guid.NewGuid(), new ExportCsvDto());

        // Assert
        result.Should().NotBeNull();
    }
}

/// <summary>
/// Integration tests for review status service.
/// </summary>
public class ReviewStatusServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IReviewStatusService _reviewStatusService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ReviewStatusServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _reviewStatusService = factory.Services.GetRequiredService<IReviewStatusService>();
        if(_reviewStatusService == null)
        {
            throw new Exception("ReviewService was null damnit");
        }
    }

    /// <summary>
    /// Test: Get review status should return Successful.
    /// </summary>
    [Fact]
    public async Task GetReviewStatusAsync_ShouldReturnSuccessful()
    {
        // Arrange
       

        // Act
        var result = await _reviewStatusService.GetReviewStatusAsync(Guid.NewGuid());

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
       

        // Act
        var result = await _reviewStatusService.ApproveContentAsync(Guid.NewGuid(), new ApproveContentDto
        {
            Status = 1,
            ReviewComments = "Approved"
        });

        // Assert
        result.Successful.Should().BeTrue();
    }
}