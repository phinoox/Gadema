// =============================================================================
// Gadema.Tests - Unit Tests for Service Logic
// =============================================================================

namespace Gadema.Tests.Services;

using Xunit;
using FluentAssertions;
using Moq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Gadema.Api.Services;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Dtos.Projects;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Enums;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Dtos.Export;
using Gadema.Core.Dtos.Reviews;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Integration tests for authentication service.
/// </summary>
using Gadema.Api.Services.Authentication;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Gadema.Core.Models.Projects;

public class AuthIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly EmailPasswordAuthService _emailAuth;
    private readonly TwoFactorAuthService _twoFactor;
    private readonly GoogleOAuthService _google;
    private User _user;
    private string _password = "Password123!";

    public void SeedAuthTestData()
    {
       

        // Seed a test user with email/password
        _user = new User
        {
            Id = Guid.NewGuid(),
            Email = "seed@example.com",
            UserName = "seeduser",
            FullName = "Seed User",
            PasswordHash = PasswordHasher.Hash(_password),
            TwoFactorEnabled = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var link = new UserProviderLink { UserId = _user.Id, Provider = UserAuthProviderEnum.Password };

        //db.Users.Add(_user);
        //db.UserProviderLinks.Add(link);
        //db.SaveChanges();
    }

    public AuthIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory  = factory;
        _emailAuth = factory.GetScopedService<EmailPasswordAuthService>();
        _twoFactor = factory.GetScopedService<TwoFactorAuthService>();
        _google    = factory.GetScopedService<GoogleOAuthService>();
        _factory.ResetDb();
        SeedAuthTestData();
    }

    [Fact]
    public void Register_ShouldSucceed_ForNewEmail()
    {
        var result = _emailAuth.Register(new RegisterDto
        {
            Email = _user.Email ,
            Password = _password,
            Name = _user.UserName
        });
        result.Successful.Should().BeTrue();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Register_ShouldConflict_ForDuplicateEmail()
    {
        var email = _user.Email;
        _emailAuth.Register(new RegisterDto { Email = email, Password = "x", Name = "A" });

        var result = _emailAuth.Register(new RegisterDto { Email = email, Password = "y", Name = "B" });
        result.Successful.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public void SignIn_ShouldFail_ForWrongPassword()
    {
        var email = _user.Email;
        _emailAuth.Register(new RegisterDto { Email = email, Password = "correct", Name = "T" });

        var result = _emailAuth.SignIn(new SignInDto { Email = email, Password = "wrong" });
        result.Successful.Should().BeFalse();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void SignIn_ShouldSucceed_ForValidCredentials()
    {
        var email = "ficker" +_user.Email;
        _emailAuth.Register(new RegisterDto { Email = email, Password = "correct", Name = "T" });

        var result = _emailAuth.SignIn(new SignInDto { Email = email, Password = "correct" });
        result.Successful.Should().BeTrue();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GoogleSignIn_WithInvalidToken_ShouldFail()
    {
        var result = _google.SignIn(new GoogleSignInDto { IdToken = "garbage" });
        result.Successful.Should().BeFalse();
    }
}

/// <summary>
/// Integration tests for project service.
/// </summary>
public class ProjectServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly IProjectService _projectService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ProjectServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _projectService = factory.GetScopedService<IProjectService>();
        
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
    private readonly ApiWebApplicationFactory _factory;
    private readonly IContentService _contentItemService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ContentItemServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _contentItemService = factory.GetScopedService<IContentService>();
        _factory.ResetDb();
        
    }

    /// <summary>
    /// Test: Create content item should return Successful.
    /// </summary>
    [Fact]
    public async Task CreateContentItemAsync_ShouldReturnSuccessful()
    {
        var scope = _factory.GetScope();

        // Ancestor path for ContentItem — explicit, in FK order:
        // Project.OwnerId is an FK to Projects (self-reference), so seed a parent first.
        var owner  =     DbSeeder.Seed<User>(scope);
        var project       = DbSeeder.Create<Project>(p => p.Owner = owner);
        var seededProject = DbSeeder.Seed(scope, project);
        //var testItem = DbSeeder.Create<ContentItem>(ci => ci.Project = seededProject);
        //var testCharacter = DbSeeder.Create<CharacterDetails>(cd => cd.ContentItem = testItem);
        //testCharacter = DbSeeder.Seed<CharacterDetails>(scope,testCharacter);
        //var item = DbSeeder.Seed<ContentItem>(scope,testItem);

        var result = await _contentItemService.CreateContentItemAsync(new CreateContentItemDto
        {
            ProjectId = seededProject.Id,
            //Project = seededProject,
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "d",
            ShortDesc = "s"
        });

        result.Successful.Should().BeTrue();


    }

    /// <summary>
    /// Test: Get content item should return Successful.
    /// </summary>
    [Fact]
    public async Task GetContentItemAsync_ShouldReturnSuccessful()
    {
        var projectId = Guid.NewGuid();
        // arrange: ensure project exists if FK required, else skip
        var created = await _contentItemService.CreateContentItemAsync(new CreateContentItemDto { ProjectId = projectId, ContentType = ContentTypeEnum.Character, Title = "C", Slug = "c", Description = "d", ShortDesc = "s" });
        created.Successful.Should().BeTrue();

        // act — need the id; if result.Data exposes Id use it
        var fetched = await _contentItemService.GetContentItemAsync(created.Data!.Id.Value, ViewModeEnum.PrivateWriting);

        // assert real behavior
        fetched.Successful.Should().BeTrue();
        fetched.Data!.Title.Should().Be("C");
    }

    [Fact]
    public async Task GetContentItemAsync_ReturnsNotFound_ForUnknownId()
    {
        var result = await _contentItemService.GetContentItemAsync(Guid.NewGuid(), ViewModeEnum.PrivateWriting);
        result.Successful.Should().BeFalse(); // a random id should NOT be "successful"
    }

    /// <summary>
    /// Test: Delete content item should return Successful.
    /// </summary>
    [Fact]
    public async Task DeleteContentItemAsync_ShouldReturnSuccessful()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        // arrange: ensure project exists if FK required, else skip
        var created = await _contentItemService.CreateContentItemAsync(new CreateContentItemDto { ProjectId = projectId, ContentType = ContentTypeEnum.Character, Title = "C", Slug = "c", Description = "d", ShortDesc = "s" });
        created.Successful.Should().BeTrue();


        // Act
        var contentItemId = created.Data == null ? new Guid() : created.Data.Id;
        var result = await _contentItemService.DeleteContentItemAsync(contentItemId.Value);

        // Assert
        result.Successful.Should().BeTrue();


    }
}

/// <summary>
/// Integration tests for task service.
/// </summary>
public class TaskServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly IProjectTaskService _projectTaskService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public TaskServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _projectTaskService = factory.GetScopedService<IProjectTaskService>();
        _factory.ResetDb();
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
     
        //Arrange
        
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
    private readonly ApiWebApplicationFactory _factory;
    private readonly IExportService _exportService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ExportServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _exportService = factory.GetScopedService<IExportService>();
        _factory.ResetDb();
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
    private readonly ApiWebApplicationFactory _factory;
    private readonly IReviewStatusService _reviewStatusService;

    /// <summary>
    /// Setup test environment.
    /// </summary>
    public ReviewStatusServiceIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _reviewStatusService = factory.GetScopedService<IReviewStatusService>();
        if(_reviewStatusService == null)
        {
            throw new Exception("ReviewService was null damnit");
        }
        _factory.ResetDb();
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