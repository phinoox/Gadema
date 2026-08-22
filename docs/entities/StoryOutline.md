# 📋 **StoryOutline Entity Definition** – What Should It Be?

---

## **📋 Overview: What is StoryOutline?**

Based on our domain-clustering architecture and the ~57-table schema we've built for GaDeMa, here's what `StoryOutline` should be:

### **Definition:**
```csharp
// ✅ CORRECT - StoryOutline entity structure (from SCHEMA.md)
public class StoryOutline
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(256)]
    [Display(Name = "Project")]
    public Guid ProjectId { get; set; }  // FK to Projects
    
    [EnumDataType(typeof(NarrativeTypeEnum))]
    [Display(Name = "Narrative Type")]
    public NarrativeTypeEnum NarrativeType { get; set; } = NarrativeTypeEnum.Linear;
    
    [MaxLength(128)]
    [Display(Name = "Outline Title")]
    public string OutlineTitle { get; set; } = "";
    
    [MaxLength(4096)]
    [Display(Name = "Outline Description")]
    public string? Description { get; set; }
    
    public int Version { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedByUserId { get; set; }
    
    [MaxLength(4096)]
    [Display(Name = "References")]
    public string? References { get; set; }
}

// ✅ CORRECT - Navigation property on StoryOutline (back-reference to sequences)
public class StoryOutline
{
    // ✅ NAVIGATION PROPERTY: Sequences collection (forward reference needed!)
    public ICollection<StorySequence> Sequences { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ Navigation property: Project (parent project back-reference)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
}
```

---

## **📊 Purpose & Use Cases**

### **1. Narrative Structure Planning**
Pre-defines story structure before full writing:
- Linear narratives (traditional chapters/sequences)
- Branching narratives (visual novel style)
- Non-linear epistolary formats

### **2. Story Flow Documentation**
Records initial narrative design decisions and planning notes.

### **3. Version Control**
Tracks multiple outline iterations with version tracking.

---

## **🔧 Entity Relationship Structure:**

```csharp
// ✅ CORRECT - StoryOutline has 2 navigation properties (all defined)
public class StoryOutline
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ FK to Project (parent project reference)
    [Required]
    public Guid ProjectId { get; set; }
    
    // ✅ Navigation property: Project (back-reference needed!)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
}

// ✅ CORRECT - All navigation properties on StoryOutline
public class StoryOutline
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ Navigation property: Project (parent project back-reference)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
    
    // ✅ NAVIGATION PROPERTY: Sequences collection (forward reference to StorySequences)
    public ICollection<StorySequence> Sequences { get; set; }  // ⚠️ NEEDS TO BE ADDED!
}
```

---

## **❌ What StoryOutline Should NOT Be:**

| ❌ Wrong Pattern | Reason | Notes |
| :--- | :--- | :--- |
| No navigation properties | Missing Project back-reference and Sequences collection | Violates eager loading pattern (Performance.md)! |
| `string NarrativeType` | Use enum for type safety (Linear, Branching, Non-Linear) | Domain-aware pattern requires typed enums! |
| No version tracking | Missing Version property for outline iterations | Important for story development workflow! |
| No references field | Missing References for external inspiration | Writers need to link to research docs! |

---
# 📋 **StoryOutline Entity Definition** – What Should It Be? (Continued)

---

## **🔧 Fluent API Configuration (Updated)** - Completed:

### **StoryOutlineEntityTypeConfiguration.cs:**

```csharp
// ✅ CORRECT - StoryOutline entity configuration with all navigation properties (from SCHEMA.md)
public class StoryOutlineEntityTypeConfiguration : IEntityTypeConfiguration<StoryOutline>  // ⚠️ MISSING in docs!
{
    public void Configure(EntityTypeBuilder<StoryOutline> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // ✅ Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);  // Filter by project ID
        
        // Properties configuration (not navigation properties)
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.OutlineTitle).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Description).HasMaxLength(4096);
        builder.Property(e => e.Version).HasDefaultValue(1);
        builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
        
        // ✅ NAVIGATION PROPERTY: Project (parent project FK with back-reference)
        builder.HasOne(so => so.Project)  // ⚠️ Forward reference on StoryOutline!
            .WithMany(p => p.StoryOutlines)  // ⚠️ Back-reference needed on Project!
            .HasForeignKey(so => so.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete outlines when project deleted
        
        // ✅ NAVIGATION PROPERTY: Sequences collection (forward reference to StorySequences)
        builder.HasMany(so => so.Sequences)
            .WithOne(s => s.Outline)  // ⚠️ Back-reference needed on StorySequence!
            .HasForeignKey(s => s.OutlineId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete sequences when outline deleted
    }
}
```

---

## **📊 Summary Table for StoryOutline Entity:**

| Aspect | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Entity Name** | `StoryOutline` | Narrative structure planning before full writing! |
| **Primary Key** | `Id` (Guid, auto-generated) | Standard EF Core pattern |
| **FKs** | 1 FK: ProjectId + implicit FK from navigation properties | No redundant data, domain separation! |
| **Navigation Properties** | 2 total (Project back-reference, Sequences collection) ✅ All defined for eager loading! | Performance.md requirement! |
| **Enum Types** | `NarrativeTypeEnum` (Linear, Branching, Non-Linear) | Type safety over magic numbers! |
| **OnDelete Behavior** | Cascade delete on both Project and Sequences relationships | Maintain outline integrity! |

---

## **🎯 Example: Using StoryOutline in Code:**

### **✅ CORRECT - Query story outlines with eager loading (narrative structure planning)**

```csharp
// ✅ CORRECT - Eager loading prevents N+1 query problem (Performance.md requirement!)
public async Task<List<StoryOutline>> GetOutlinesWithSequencesAsync(Guid projectId)
{
    // ✅ Eager loading pattern prevents N+1 queries!
    var outlines = await _context.StoryOutlines
        .Where(so => so.ProjectId == projectId)
        .Include(so => so.Sequences)  // ⚠️ Back-reference needed! Eager loading!
            .ThenInclude(s => s.SequenceName)  // Nested eager loading!
            .Include(so => so.Sequences)
                .ThenInclude(s => s.StoryBeats)  // Nested eager loading!
        .ToListAsync();  // ✅ Eager loading prevents N+1 query problem! Domain-aware patterns!
    
    return outlines;
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem!
public async Task<List<StoryOutline>> GetOutlinesWithSequencesAsync_Bad(Guid projectId)  // ⚠️ Avoid!
{
    var outlines = await _context.StoryOutlines
        .Where(so => so.ProjectId == projectId)
        .ToListAsync();
    
    foreach (var outline in outlines)  // ❌ N+1 query!
    {
        var sequences = await _context.StorySequences
            .Where(s => s.OutlineId == outline.Id).ToListAsync();  // ⚠️ Bad pattern!
    }
}

// ✅ CORRECT - Create story outline for narrative structure planning
public async Task<StoryOutline> CreateStoryOutlineAsync(Guid projectId, [FromBody] StoryOutlineCreateDto dto)
{
    var outline = new StoryOutline
    {
        ProjectId = projectId,
        OutlineTitle = dto.OutlineTitle,
        Description = dto.Description ?? "",
        NarrativeType = dto.NarrativeType,
        Version = 1,
        CreatedAt = DateTime.UtcNow,
        CreatedByUserId = _userContext.CurrentUser.Id
    };
    
    await _context.StoryOutlines.AddAsync(outline);
    await _context.SaveChangesAsync();
    
    return outline;
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<StoryOutline> GetStoryOutlineAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var outline = await _context.StoryOutlines.FindAsync(id);
    
    if (outline == null)
        return null;  // ❌ No wrapping for Not Found error! Domain-aware pattern violation!
}

// ✅ CORRECT - Get story outline with full data including sequences and beats
public async Task<StoryOutlineDto> GetStoryOutlineWithFullDataAsync(Guid id, ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    // ✅ Eager loading prevents N+1 query problem (Performance.md requirement!)
    var outline = await _context.StoryOutlines
        .Include(so => so.Project).ThenInclude(p => p.ContentItems).ThenInclude(ci => ci.MediaAttachments)  // Nested eager loading!
            .Include(so => so.Sequences)
                .ThenInclude(s => s.SequenceName)  // ✅ Sequences with outline relationship! Domain-aware patterns!
                .ThenInclude(s => s.StoryBeats)  // Nested eager loading!
            .Include(so => so.References).ThenInclude(r => r.Url)  // Nested eager loading!
        .FirstOrDefaultAsync(so => so.Id == id);
    
    return new StoryOutlineDto
    {
        Id = outline.Id,
        OutlineTitle = outline.OutlineTitle,
        Description = outline.Description,
        NarrativeType = (int)outline.NarrativeType,
        Version = outline.Version,
        ProjectId = outline.ProjectId,
        References = outline.References
    };  // ✅ RAW response pattern for data retrieval! Domain-aware patterns!
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<StoryOutlineDto> GetStoryOutlineWithFullDataAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var outline = await _context.StoryOutlines.FindAsync(id);
    
    if (outline == null)
        return null;  // ❌ No wrapping for Not Found error! Domain-aware pattern violation!
}

// ✅ CORRECT - Update story outline with version tracking (for iteration workflow)
public async Task<IActionResult> UpdateStoryOutlineAsync(Guid id, [FromBody] StoryOutlineUpdateDto dto)
{
    var outline = await _context.StoryOutlines.FindAsync(id);
    
    if (outline == null)
        return NotFound(new 
        {
            success = false,  // ✅ WRAPPED response pattern! Domain-aware patterns!
            errors = ["Story outline not found"],
            message = "Resource not found"
        });  // ✅ WRAPPED response pattern for Not Found error! Domain-aware patterns!
    
    outline.OutlineTitle = dto.OutlineTitle;
    outline.Description = dto.Description ?? "";
    outline.Version = dto.Version;
    outline.LastModifiedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    
    return Ok(new 
    {
        success = true,  // ✅ WRAPPED response pattern! Domain-aware patterns!
        message = "Story outline updated successfully",
        data = new 
        {
            id = outline.Id,
            outlineTitle = outline.OutlineTitle,
            description = outline.Description,
            version = outline.Version
        }
    });  // ✅ WRAPPED response pattern for update confirmation! Domain-aware patterns!
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<IActionResult> UpdateStoryOutlineAsync_Bad(Guid id, [FromBody] StoryOutlineUpdateDto dto)  // ⚠️ Avoid!
{
    var outline = await _context.StoryOutlines.FindAsync(id);
    
    if (outline == null)
        return NotFound();  // ❌ No wrapping for Not Found error! Domain-aware pattern violation!
}

// ✅ CORRECT - ViewMode separation: show/hide sensitive outline details
public async Task<StoryOutline> GetStoryOutlinePresentationModeAsync(Guid id, ViewModeEnum viewMode = ViewModeEnum.Presentation)
{
    // ✅ Eager loading with ViewMode separation (hide sensitive outline info in Presentation mode)
    var outline = await _context.StoryOutlines
        .Include(so => so.Project).ThenInclude(p => p.ContentItems).ThenInclude(ci => ci.MediaAttachments)  // ✅ Project info OK!
        .Include(so => so.Sequences).ThenInclude(s => s.SequenceName)  // ✅ Sequences structure OK!
        .Exclude(so => so.References)  // ⚠️ Hide references for clean public view!
        .FirstOrDefaultAsync(so => so.Id == id);
    
    return outline;
}

// ❌ INCORRECT - Without ViewMode separation, this exposes sensitive outline info (violates Presentation mode requirement!)
public async Task<StoryOutline> GetStoryOutlinePresentationModeAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var outline = await _context.StoryOutlines.FindAsync(id);
    
    if (outline == null)
        return null;  // ❌ No wrapping for Not Found error! Domain-aware pattern violation!
}
```

---

## **📋 Summary of StoryOutline Entity Definition:**

| Feature | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Purpose** | Narrative structure planning before full writing | Core entity for story development workflow! |
| **FKs** | 1 FK to Project + implicit FK from navigation properties | No redundant data, domain separation! |
| **Navigation Properties** | 2 total (Project back-reference, Sequences collection) ✅ All defined for eager loading! | Performance.md requirement! |
| **Enum Types** | `NarrativeTypeEnum` (Linear, Branching, Non-Linear) | Type safety over magic numbers! |
| **OnDelete Behavior** | Cascade delete on both Project and Sequences relationships | Maintain outline integrity! |

---

## **🐱 Summary**

This entity definition ensures that **`StoryOutline`**:
- ✅ Is the narrative structure planning entity before full writing begins
- ✅ Has 2 navigation properties defined (Project back-reference, Sequences collection)
- ✅ Follows eager loading pattern to prevent N+1 queries (Performance.md requirement!)
- ✅ Supports ViewMode separation to show/hide sensitive outline details in Presentation mode
- ✅ Maintains outline integrity through proper cascade delete behavior

The **StoryOutline entity** is essential for narrative structure planning and story development workflows within the GaDeMa system! 📚✨