// =============================================================================
// GameDev.Tests - Integration Tests for API Endpoints
// =============================================================================

namespace GameDev.Tests.Integration;

using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using GameDev.Core.Dtos.Projects;
using System.Net.Http.Json;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Enums;
using GameDev.Core.Dtos.Tasks;
using GameDev.Core.Dtos.Export;
using GameDev.Core.Dtos.ExternalReferences;
using Microsoft.Extensions.Hosting;
using GameDev.Data;

/// <summary>
/// Integration tests for API endpoints using WebApplicationFactory.
/// </summary>
public class ApiIntegrationTests
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ApiIntegrationTests()
    {
         using var host = new HostBuilder()
        .ConfigureWebHost(builder =>
        {
            builder.UseTestServer()
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
        })
        .Build();

    _httpClient = host.GetTestClient();
    }

    /// <summary>
    /// Test: Create project should return 200 OK.
    /// </summary>
    [Fact]
    public async Task CreateProject_ShouldReturnOk()
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "Test Project",
            Visibility = 1
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/projects", createProjectDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: List projects should return 200 OK.
    /// </summary>
    [Fact]
    public async Task GetProjects_ShouldReturnOk()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/projects");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Create content item should return 200 OK.
    /// </summary>
    [Fact]
    public async Task CreateContentItem_ShouldReturnOk()
    {
        // Arrange
        var createContentDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test character description",
            ShortDesc = "Test character"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/content/items", createContentDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Create task should return 200 OK.
    /// </summary>
    [Fact]
    public async Task CreateTask_ShouldReturnOk()
    {
        // Arrange
        var createTaskDto = new ProjectTaskCreateDto
        {
            ProjectId = Guid.NewGuid(),
            TaskTitle = "Test Task",
            Status = 0,
            Priority = 1,
            Difficulty = 0,
            IsQuickWin = true
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Export to JSON should return 200 OK.
    /// </summary>
    [Fact]
    public async Task ExportToJson_ShouldReturnOk()
    {
        // Arrange
        var exportJsonDto = new ExportJsonDto
        {
            ProjectId = Guid.NewGuid(),
            Sections = "[]",
            IncludeWatermark = false
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/projects/{id}/export/json", exportJsonDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// Test: Create external reference should return 200 OK.
    /// </summary>
    [Fact]
    public async Task CreateExternalReference_ShouldReturnOk()
    {
        // Arrange
        var createReferenceDto = new CreateExternalReferenceDto
        {
            ContentItemId = Guid.NewGuid(),
            Url = "https://example.com",
            Type = 0
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/content-items/{id}/references", createReferenceDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}