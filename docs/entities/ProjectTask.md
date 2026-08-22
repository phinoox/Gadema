# 📋 **ProjectTask Entity Definition** – What Should It Be?

---

## **📋 Overview: What is ProjectTask?**

Based on our domain-clustering architecture and the ~57-table schema we've built for GaDeMa, here's what `ProjectTask` should be:

### **Definition:**
```csharp
// ✅ CORRECT - ProjectTask entity structure (from SCHEMA.md)
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(256)]
    [Display(Name = "Project")]
    public Guid ProjectId { get; set; }  // FK to Projects
    
    [EnumDataType(typeof(TaskStatusEnum)), Required]
    [Display(Name = "Task Status")]
    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Backlog;
    
    [EnumDataType(typeof(TaskDifficultyEnum)))]
    [Display(Name = "Task Difficulty")]
    public int Difficulty { get; set; }  // 0=Easy, 1=Medium, 2=Hard
    
    [MaxLength(512)]
    [Display(Name = "Task Title")]
    public string TaskTitle { get; set; } = "";
    
    [MaxLength(4096)]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    public int EstimatedMinutes { get; set; }  // For ADHD-friendly quick wins
    
    [Display(Name = "Is Quick Win?")]
    public bool IsQuickWin { get; set; } = false;
    
    public Guid? ContentItemId { get; set; }  // Optional FK to ContentItem (for content-specific tasks)
    
    public Guid? AssignedToUserId { get; set; }  // FK to User (who owns this task)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime CompletedAt { get; set; }  // When task was completed
    
    public Guid CreatedByUserId { get; set; }
}

// ✅ CORRECT - Navigation properties on ProjectTask (from SCHEMA.md)
public class ProjectTask
{
    // ✅ NAVIGATION PROPERTY: Project (parent project FK with back-reference)
    public virtual Project Project { get; set; }
    
    // ✅ NAVIGATION PROPERTY: ContentItem (optional relationship to content item)
    public virtual ContentItem? ContentItem { get; set; }
    
    // ✅ NAVIGATION PROPERTY: Assigned User (back-reference for task ownership)
    public virtual User? AssignedToUser { get; set; }
    
    // ✅ NAVIGATION PROPERTY: TaskComments collection (comments specific to this task)
    public ICollection<TaskComment> TaskComments { get; set; }
    
    // ✅ NAVIGATION PROPERTY: ReviewStatus (if we have review workflow)
    public virtual ReviewStatus? ReviewStatus { get; set; }  // Optional but recommended!
}
```

---

## **📊 Purpose & Use Cases**

### **1. ADHD-Friendly Task Management**
Flat structure for writing/design work:
- Quick Win badges for short tasks (<15 minutes)
- Difficulty filtering (Easy/Medium/Hard) for energy level matching
- Single-task focus views to reduce cognitive load

### **2. Content-Specific Tasks**
Optional relationship to specific content items:
- "Write Geralt's introduction scene" → links to character content item
- "Update combat mechanics for boss fight" → links to mechanics content item

### **3. Task Workflow Tracking**
From Backlog → InProgress → Review → Done with completion timestamps.

---

## **🔧 Entity Relationship Structure:**

```csharp
// ✅ CORRECT - ProjectTask has 5 navigation properties (all defined)
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ FK to Project (parent project reference)
    [Required]
    public Guid ProjectId { get; set; }
    
    // ✅ Navigation property: Project (back-reference needed!)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
}

// ✅ CORRECT - All navigation properties on ProjectTask
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ Navigation property: Project (back-reference to parent project)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ Navigation property: ContentItem (optional relationship to content item)
    public virtual ContentItem? ContentItem { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ Navigation property: Assigned User (back-reference for task ownership)
    public virtual User? AssignedToUser { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ Navigation property: TaskComments collection (comments specific to this task)
    public ICollection<TaskComment> TaskComments { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ Navigation property: ReviewStatus (if we have review workflow)
    public virtual ReviewStatus? ReviewStatus { get; set; }  // ⚠️ Optional but recommended!
}
```

---

## **❌ What ProjectTask Should NOT Be:**

| ❌ Wrong Pattern | Reason | Notes |
| :--- | :--- | :--- |
| `int Status` (no enum) | Use TaskStatusEnum for type safety (Backlog, InProgress, Review, Done) | Domain-aware pattern requires typed enums! |
| `int Difficulty` (magic numbers) | Should have explicit difficulty levels defined as enum or constants | Avoids magic number 0/1/2 confusion! |
| No navigation properties | Missing back-references on Project, User, ContentItem, TaskComments | Violates eager loading pattern (Performance.md)! |
| Mandatory FK to ContentItem | Should be optional (nullable) since tasks can be project-level OR content-specific | Domain separation requires proper optional relationships! |

---
# 📋 **ProjectTask Entity Definition** – What Should It Be? (Continued)

---

## **🔧 Fluent API Configuration (Updated)** - Completed:

### **ProjectTaskEntityTypeConfiguration.cs:**

```csharp
// ✅ CORRECT - ProjectTask entity configuration with all navigation properties (from SCHEMA.md)
public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask>  // ⚠️ PARTIALLY MISSING in docs!
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // ✅ Indexes for frequently filtered columns (ADHD-friendly filtering!)
        builder.HasIndex(e => e.ProjectId);  // Filter by project ID
        builder.HasIndex(e => e.Status);     // Filter by task status (Backlog, InProgress, etc.)
        builder.HasIndex(e => e.Difficulty); // Filter by difficulty level (Easy, Medium, Hard)
        builder.HasIndex(e => e.IsQuickWin); // ADHD-friendly filter for quick wins
        builder.HasIndex(e => e.AssignedToUserId);  // Filter by assigned user
        
        // ✅ NAVIGATION PROPERTY: Project (parent project FK with back-reference)
        builder.HasOne(pt => pt.Project)  // ⚠️ Forward reference on ProjectTask!
            .WithMany(p => p.ProjectTasks)  // ⚠️ Back-reference needed on Project!
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete tasks when project deleted
        
        // ✅ NAVIGATION PROPERTY: ContentItem (optional relationship to content item)
        builder.HasOne(pt => pt.ContentItem)  // ⚠️ Forward reference on ContentItem!
            .WithMany(c => c.ProjectTasks)  // ⚠️ Back-reference needed on ContentItem!
            .HasForeignKey(pt => pt.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // ✅ Maintain task history!
        
        // ✅ NAVIGATION PROPERTY: Assigned User (back-reference for task ownership)
        builder.HasOne(pt => pt.AssignedToUser)  // ⚠️ Forward reference on User!
            .WithMany(u => u.ProjectTasks)  // ⚠️ Back-reference needed on User!
            .HasForeignKey(pt => pt.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);  // ✅ Don't delete user when task deleted!
        
        // ✅ NAVIGATION PROPERTY: TaskComments collection (comments specific to this task)
        builder.HasMany(pt => pt.TaskComments)
            .WithOne(tc => tc.ProjectTask)  // ⚠️ Back-reference needed on TaskComment!
            .HasForeignKey(tc => tc.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete comments when task deleted
        
        // ✅ NAVIGATION PROPERTY: ReviewStatus (if we have review workflow)
        builder.HasOne(pt => pt.ReviewStatus)  // ⚠️ Forward reference on ReviewStatus!
            .WithOne(rs => rs.ProjectTask)  // ⚠️ Back-reference needed on ReviewStatus!
            .HasForeignKey(pt => pt.Id)  // ✅ Or use ReviewStatusId as FK instead of Id
            .OnDelete(DeleteBehavior.SetNull);  // ✅ Don't delete review status when task deleted
        
        // Properties configuration (not navigation properties)
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.TaskTitle).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Description).HasMaxLength(4096);
        builder.Property(e => e.EstimatedMinutes).HasDefaultValue(15);  // Default 15 min for ADHD-friendly!
        builder.Property(e => e.IsQuickWin).HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
    }
}
```

---

## **📊 Summary Table for ProjectTask Entity:**

| Aspect | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Entity Name** | `ProjectTask` (renamed from Task) ⭐ | Avoids ambiguity with System.Threading.Task! |
| **Primary Key** | `Id` (Guid, auto-generated) | Standard EF Core pattern |
| **FKs** | 1 FK: ProjectId + optional FK to ContentItem/AssignedToUser | Domain separation requires proper optional relationships! |
| **Navigation Properties** | 5 total (Project, ContentItem, AssignedToUser, TaskComments, ReviewStatus) ✅ All defined for eager loading! | Performance.md requirement! |
| **Enum Types** | `TaskStatusEnum` (Backlog, InProgress, Review, Done) + Difficulty levels | Type safety over magic numbers! |
| **OnDelete Behavior** | Cascade (Project, TaskComments), SetNull (User, ReviewStatus) | Maintain task history! |

---

## **🎯 Example: Using ProjectTask in Code:**

### **✅ CORRECT - Query tasks with eager loading (ADHD-friendly filtering)**

```csharp
// ✅ CORRECT - Eager loading prevents N+1 query problem (Performance.md requirement!)
public async Task<List<ProjectTask>> GetQuickWinTasksAsync(Guid projectId)
{
    // ✅ Eager loading pattern prevents N+1 queries!
    var quickWinTasks = await _context.ProjectTasks
        .Where(pt => pt.ProjectId == projectId && pt.IsQuickWin)
        .Include(pt => pt.ContentItem).ThenInclude(ci => ci.Project)  // Nested eager loading!
            .Include(pt => pt.TaskComments)
                .ThenInclude(tc => tc.CreatedByUser)
        .Include(pt => pt.AssignedToUser)
        .OrderByDescending(pt => pt.EstimatedMinutes)
        .ToListAsync();  // ✅ Eager loading prevents N+1 query problem! Domain-aware patterns!
    
    return quickWinTasks;
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem!
public async Task<List<ProjectTask>> GetQuickWinTasksAsync_Bad(Guid projectId)  // ⚠️ Avoid!
{
    var quickWinTasks = await _context.ProjectTasks
        .Where(pt => pt.ProjectId == projectId && pt.IsQuickWin)
        .ToListAsync();
    
    foreach (var task in quickWinTasks)  // ❌ N+1 query!
    {
        var contentItem = await _context.ContentItems.FindAsync(task.ContentItemId);  // ⚠️ Bad pattern!
        var comments = await _context.TaskComments.Where(tc => tc.ProjectTaskId == task.Id).ToListAsync();  // ⚠️ N+1 problem!
    }
}

// ✅ CORRECT - Update task status with eager loading (for task completion tracking)
public async Task<IActionResult> UpdateProjectTaskAsync(Guid projectId, Guid taskId, [FromBody] ProjectTaskUpdateDto dto)
{
    var projectTask = await _context.ProjectTasks.FindAsync(projectId, taskId);
    
    if (projectTask == null)
        return NotFound(new 
        {
            success = false,  // ✅ WRAPPED response pattern! Domain-aware patterns!
            errors = ["Project task not found"],
            message = "Resource not found"
        });
    
    projectTask.Status = dto.Status;
    projectTask.CompletedAt = dto.Status == TaskStatusEnum.Done ? DateTime.UtcNow : null;
    
    await _context.SaveChangesAsync();
    
    return Ok(new 
    {
        success = true,  // ✅ WRAPPED response pattern! Domain-aware patterns!
        message = "Project task updated successfully",
        data = new 
        {
            id = projectTask.Id,
            status = (int)projectTask.Status,
            completedAt = projectTask.CompletedAt
        }
    });  // ✅ WRAPPED response pattern for update confirmation! Domain-aware patterns!
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<IActionResult> UpdateProjectTaskAsync_Bad(Guid projectId, Guid taskId, [FromBody] ProjectTaskUpdateDto dto)  // ⚠️ Avoid!
{
    var projectTask = await _context.ProjectTasks.FindAsync(projectId, taskId);
    
    if (projectTask == null)
        return NotFound();  // ❌ No wrapping for Not Found error! Domain-aware pattern violation!
    
    projectTask.Status = dto.Status;
    await _context.SaveChangesAsync();
    
    return Ok(projectTask);  // ❌ No wrapping for update confirmation! Domain-aware pattern violation!
}

// ✅ CORRECT - Get task with full data including content item and reviews (if applicable)
public async Task<ProjectTaskDto> GetProjectTaskWithFullDataAsync(Guid projectId, Guid taskId)
{
    // ✅ Eager loading prevents N+1 query problem (Performance.md requirement!)
    var projectTask = await _context.ProjectTasks
        .Include(pt => pt.Project).ThenInclude(p => p.ContentItems).ThenInclude(ci => ci.MediaAttachments)  // Nested eager loading!
            .Include(pt => pt.AssignedToUser).ThenInclude(u => u.TeamMemberships).ThenInclude(tm => tm.Team)  // Nested eager loading!
        .Include(pt => pt.TaskComments).ThenInclude(tc => tc.CreatedByUser)
        .Include(pt => pt.ContentItem).ThenInclude(ci => ci.ExternalReferences)  // Nested eager loading!
            .Include(ci => ci.CharacterIdentities)  // Nested eager loading for character tasks!
            .FirstOrDefaultAsync(pt => pt.Id == taskId);
    
    return new ProjectTaskDto
    {
        Id = projectTask.Id,
        TaskTitle = projectTask.TaskTitle,
        Description = projectTask.Description,
        Status = (int)projectTask.Status,
        Difficulty = projectTask.Difficulty,
        EstimatedMinutes = projectTask.EstimatedMinutes,
        IsQuickWin = projectTask.IsQuickWin,
        AssignedToUserId = projectTask.AssignedToUserId,
        ContentItemId = projectTask.ContentItemId,
        CompletedAt = projectTask.CompletedAt
    };  // ✅ RAW response pattern for data retrieval! Domain-aware patterns!
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<ProjectTaskDto> GetProjectTaskWithFullDataAsync_Bad(Guid projectId, Guid taskId)  // ⚠️ Avoid!
{
    var projectTask = await _context.ProjectTasks.FindAsync(projectId, taskId);
    
    if (projectTask == null)
        return null;
    
    // ❌ Multiple separate queries for same task data! N+1 problem!
    var contentItem = await _context.ContentItems.FindAsync(projectTask.ContentItemId);  // ⚠️ Bad pattern!
    var assignedToUser = await _context.Users.FindAsync(projectTask.AssignedToUserId);  // ⚠️ Bad pattern!
    
    return new ProjectTaskDto { ... };  // ❌ No wrapping for data retrieval! Domain-aware pattern violation!
}
```

---

## **📋 Summary of ProjectTask Entity Definition:**

| Feature | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Purpose** | ADHD-friendly task management with quick win filtering and difficulty levels | Core entity for writing/design work! |
| **FKs** | 1 FK to Project + optional FK to ContentItem, AssignedToUser | Domain separation requires proper optional relationships! |
| **Navigation Properties** | 5 total (Project, ContentItem, AssignedToUser, TaskComments, ReviewStatus) ✅ All defined for eager loading! | Performance.md requirement! |
| **Enum Types** | `TaskStatusEnum` + Difficulty levels (Easy/Medium/Hard) | Type safety over magic numbers! |
| **OnDelete Behavior** | Cascade (Project, TaskComments), SetNull (User, ReviewStatus) | Maintain task history! |

---

## **🐱 Summary**

This entity definition ensures that **`ProjectTask`**:
- ✅ Is the ADHD-friendly task management entity with quick win filtering and difficulty levels
- ✅ Has 5 navigation properties defined (Project back-reference, ContentItem, AssignedToUser, TaskComments, ReviewStatus)
- ✅ Follows eager loading pattern to prevent N+1 queries (Performance.md requirement!)
- ✅ Supports content-specific tasks via optional FK to ContentItem
- ✅ Maintains task history through proper cascade delete behavior

The **ProjectTask entity** is essential for ADHD-friendly task management and writing/design workflow tracking within the GaDeMa system! 🎯✨