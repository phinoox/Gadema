// =============================================================================
// Gadema.Tests - Integration Tests for API Endpoints
// =============================================================================

namespace Gadema.Tests.Integration;

using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Gadema.Core.Dtos.Projects;
using System.Net.Http.Json;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Enums;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Dtos.Export;
using Gadema.Core.Dtos.ExternalReferences;
using Microsoft.Extensions.Hosting;
using Gadema.Core.Database;
using Microsoft.AspNetCore.Mvc.Testing;

/// <summary>
/// Integration tests for API endpoints using WebApplicationFactory.
/// </summary>
public class ApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    private readonly ApiWebApplicationFactory _factory;
    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ApiIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;

        if (_factory == null)
            throw new Exception("factory was null you dumbass");
        _httpClient = _factory.CreateClient();


    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        var response = await _httpClient.GetAsync("/health");

        response.IsSuccessStatusCode.Should().BeTrue();
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

        await response.Content.ReadAsStringAsync();

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
        if (_httpClient == null)
            throw new Exception("client was null you dumbass");
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