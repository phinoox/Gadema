# 🧪 Testing Guidelines for GaDeMa v0.1

This document defines testing requirements, patterns, and coverage targets for the GaDeMa application to ensure quality and maintainability.

## Coverage Targets

### Unit Tests
- **Target**: ≥80% code coverage for business logic
- **Excluded from coverage target**: External services (AI, Email, Redis)
- **Framework**: xUnit + Moq for dependency injection
- **Assertion Library**: FluentAssertions

### Integration Tests  
- **Target**: Cover critical API endpoints and database operations
- **Framework**: WebApplicationFactory with test database
- **Database**: Separate in-memory SQLite for tests
- **Coverage**: CRUD operations, authentication flows, pagination

---

## Unit Test Structure

### Arrange-Act-Assert Pattern

```csharp
public class ContentItemServiceTests : IDisposable
{
    private readonly GameDbContext _context;
    private readonly Mock<IEmailService> _emailMock;
    
    public ContentItemServiceTests()
    {
        _emailMock = new Mock<IEmailService>();
        // ... setup DbContext with test data
    }
    
    [Fact]
    public async Task CreateContentAsync_WhenDescriptionIsNull_ShouldThrowValidationException()
    {
        // Arrange: Setup test data with null description
        var contentDto = new ContentItemDto 
        {
            Title = "Test Character",
            Description = null,  // Invalid case
            ContentType = ContentTypeEnum.Character
        };

        // Act: Execute service method
        var exception = await Assert.ThrowsAsync<ValidationException>(
            async () => await _contentService.CreateContentAsync(contentDto, Guid.NewGuid()));

        // Assert: Verify error message
        exception.Message.Should().Contain("Description cannot be empty");
    }
    
    [Fact]
    public async Task CreateContentAsync_ValidInput_ShouldReturnCreatedItem()
    {
        // Arrange
        var validDto = new ContentItemDto 
        {
            Title = "Valid Character",
            Description = "This is a test description",
            ContentType = ContentTypeEnum.Character,
            ProjectId = Guid.NewGuid()
        };

        // Act: Create content item
        var result = await _contentService.CreateContentAsync(validDto, validDto.ProjectId);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Valid Character");
        result.Status.Should().Be(ContentStatusEnum.Draft);
    }
}
```

### Test Naming Convention

```csharp
// ✅ CORRECT - Clear test name with expected behavior
public async Task CreateContentAsync_ValidDescription_ShouldPersistData() {}
public async Task CreateContentAsync_NullTitle_ShouldThrowValidationException() {}
public async Task GetPublishedCharactersAsync_ReturnsOnlyPublishedItems() {}

// ❌ INCORRECT - Vague test names
public async Task Test1() {}  // ❌ What does this test?
public async Task Scenario() {}  // ❌ Too generic
```

---

## Integration Test Structure

### WebApplicationFactory Pattern

```csharp
public class ApiIntegrationTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;
    
    public ApiIntegrationTests(ApiTestFixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task GetPublishedContentAsync_ReturnsOnlyPublishedItems()
    {
        // Arrange: Create test data
        var contentItem = await Fixture.SetupPublishedContent();

        // Act: Call API endpoint
        var response = await _client.GetAsync("/api/v1/content/items?projectId=abc&published=true");

        // Assert: Verify published items only
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<List<ContentItemDto>>();
        content.All(c => c.Published).Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateProjectTaskAsync_ShouldCreateWithDefaultValues()
    {
        // Arrange: Setup project ID
        var projectId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Act: Create task with minimum data
        var taskDto = new ProjectTaskCreateDto 
        {
            TaskTitle = "Test Task",
            ProjectId = projectId
        };
        
        var response = await _client.PostAsJsonAsync(
            "/api/v1/projects/{id}/tasks",
            taskDto);

        // Assert: Verify successful creation
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProjectTaskDto>();
        result.Status.Should().Be(TaskStatusEnum.Backlog, "Default status");
        result.IsQuickWin.Should().BeFalse(), "Default quick win";
    }
}
```

### Database Transaction Cleanup

```csharp
[Fact]
public async Task CreateProject_ShouldRollbackOnFailure()
{
    using var scope = ApplicationServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    
    // Arrange: Start transaction
    await context.Database.BeginTransactionAsync();

    try
    {
        // Act: Attempt invalid operation
        var dto = new CreateProjectDto { Title = "" };  // Invalid title
        await _projectService.CreateAsync(dto);

        // Assert: Should not reach here (thrown earlier)
        Assert.False(true, "Should have thrown validation exception");
    }
    finally
    {
        // Cleanup: Rollback transaction
        await context.Database.RollbackTransactionAsync();
    }
}
```

---

## Mock External Services

### Email Service Mocking

```csharp
public class AuthenticationControllerTests : IDisposable
{
    private readonly Mock<IEmailService> _emailMock;
    
    public AuthenticationControllerTests()
    {
        _emailMock = new Mock<IEmailService>();
        // Setup default behavior for email sending
        _emailMock.Setup(e => e.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
    }
    
    [Fact]
    public async Task RegisterUserAsync_ShouldSendWelcomeEmail()
    {
        // Arrange
        var registerDto = new RegisterDto 
        {
            Email = "test@example.com",
            Password = "SecurePass123!"  // Valid password
        };

        // Act: Create user
        await _controller.RegisterUserAsync(registerDto);
        
        // Assert: Verify email was sent (mock verification)
        _emailMock.Verify(e => e.SendWelcomeEmail(It.IsAny<string>(), It.IsAny<string>()), 
            Times.Once, "Welcome email should be sent");
    }
}
```

### Redis Cache Mocking

```csharp
public class ContentCacheServiceTests : IDisposable
{
    private readonly Mock<IRedisCache> _cacheMock;
    
    public ContentCacheServiceTests()
    {
        _cacheMock = new Mock<IRedisCache>();
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((Task<string>)null);  // Return null for missing keys
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);
    }
    
    [Fact]
    public async Task GetSequencesWithCacheAsync_WhenCacheMiss_ShouldQueryDatabase()
    {
        // Arrange: Setup cache miss scenario
        _cacheMock.Setup(c => c.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<StorySequence>>>())));

        // Act & Assert: Verify database query is executed on cache miss
        var result = await _service.GetSequencesWithCacheAsync(projectId);
        
        // Verify cache was checked first
        _cacheMock.Verify(c => c.GetAsync(It.Is<string>(key => key.Contains("sequences"))), 
            Times.Once, "Should check cache first");
    }
}
```

---

## Test Fixture Setup

### Database Seed Method

```csharp
public static async Task SeedTestDataAsync(GameDbContext context)
{
    if (await context.Users.AnyAsync()) return;  // Don't re-seed
    
    var testUser = new User 
    {
        UserName = "testuser",
        Email = "test@example.com",
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
    
    await context.Users.AddAsync(testUser);
    await context.SaveChangesAsync();
}
```

### Test Database Configuration

```csharp
// In test project setup:
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseSqlite("Data Source=:memory:");  // In-memory database
});
```

---

## Validation Test Patterns

### Multiple Error Messages

```csharp
[Fact]
public async Task CreateContentAsync_InvalidTitleAndEmptyDescription_ShouldReturnMultipleErrors()
{
    var dto = new ContentItemDto 
    {
        Title = "",  // Invalid - empty title
        Description = null,  // Invalid - missing description
        ContentType = ContentTypeEnum.Character
    };

    try
    {
        await _service.CreateContentAsync(dto, Guid.NewGuid());
        Assert.True(false, "Should throw ValidationException");
    }
    catch (ValidationException ex)
    {
        ex.Errors.Should().Contain("Title is required", "Title validation");
        ex.Errors.Should().Contain("Description cannot be empty", "Description validation");
    }
}
```

### Range Validation Tests

```csharp
[Fact]
public async Task UpdateTaskAsync_PageSizeOutOfRange_ShouldUseDefaultValue()
{
    // Arrange
    var dto = new ProjectTaskCreateDto 
    {
        TaskTitle = "Valid Title"
    };

    // Act: Try to update with out-of-range page size
    var result = await _service.UpdateTaskAsync(projectId, taskId, dto, 500);  // Page size > 100

    // Assert: Should use default page size
    result.PageSize.Should().Be(20);  // Default value when range exceeded
}
```

---

## Performance Test Guidelines

### Load Testing API Endpoints

```csharp
[Fact]
public async Task GetPublishedContentAsync_ShouldCompleteWithin500ms()
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    
    // Act: Multiple requests to measure performance
    for (int i = 0; i < 100; i++)
    {
        await _client.GetAsync("/api/v1/content/items?projectId=abc");
    }

    watch.Stop();
    var avgTimePerRequest = watch.ElapsedMilliseconds / 100.0;

    // Assert: Should complete within acceptable time
    avgTimePerRequest.Should().BeLessThan(50, 
        $"Average response time {avgTimePerRequest:F2}ms exceeds 50ms target");
}
```

### Database Query Performance

```csharp
[Fact]
public async Task EagerLoadingQuery_ShouldExecuteAsSingleSqlStatement()
{
    // Arrange: Setup test data with relationships
    var contentItems = await _context.ContentItems.AddRangeAsync(
        CreateTestContentItem("Char1"),
        CreateTestContentItem("Char2")
    );

    foreach (var item in contentItems)
    {
        item.MediaAttachments.Add(CreateMediaAttachment(item.Id));
    }

    await _context.SaveChangesAsync();

    // Act: Execute eager loading query
    var stopwatch = Stopwatch.StartNew();
    var results = await _context.ContentItems
        .Include(c => c.MediaAttachments)
        .ToListAsync();
    stopwatch.Stop();

    // Assert: Should execute efficiently (< 100ms)
    stopwatch.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(100, 
        "Eager loading query should complete quickly");
}
```

---

## Summary: Testing Requirements Checklist

| Test Type | Coverage Target | Priority | Notes |
| :--- | :--- | :--- | :--- |
| **Unit Tests** | ≥80% business logic | High | Use xUnit + Moq |
| **Integration Tests** | Critical API endpoints | Medium | WebApplicationFactory |
| **Database Tests** | CRUD operations | Medium | Transaction cleanup |
| **Validation Tests** | Input validation rules | High | Multiple error scenarios |
| **Performance Tests** | Key API endpoints | Low | Load testing for high-traffic paths |

---

## Anti-Patterns to Avoid in Testing

| Anti-Pattern | Example | ✅ Correct Approach |
| :--- | :--- | :--- |
| **Hardcoded Test Data** | `var item = new ContentItem { Title = "Test" }` | Use setup methods with unique IDs |
| **Database Pollution** | Not cleaning up test data | Use transactions or in-memory DB |
| **External Service Calls** | Calling real email service | Mock external services |
| **Vague Test Names** | `public async Task Test1()` | `public async Task CreateAsync_ValidInput_ShouldSucceed()` |
| **Missing Assertions** | `await _service.CreateAsync(dto);` without assertions | Always verify expected behavior |

---

## Final Reminder

**Testing is essential for maintaining code quality and catching bugs early!**

- ✅ Write tests alongside features (Test-Driven Development)
- ✅ Keep test names descriptive and meaningful
- ✅ Use mock objects for external dependencies
- ✅ Clean up database state after each test
- ✅ Aim for ≥80% coverage on business logic
```
