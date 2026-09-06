# Missing Navigation Properties Analysis

## Build Errors: 114 total CS1061 errors

This document lists all navigation properties that need to be added to model classes to fix the build.

## Summary of Fixes Needed:

### 1. Model Classes Needing `[ForeignKey]` Navigation Properties (Single Relationships)
Each needs a `public virtual EntityType RelatedEntity { get; set; }` with `[ForeignKey("FKProperty")]`

- **AbilitySet**: Needs `Project` navigation property  
- **AssetLink**: Needs `MetaInfo` navigation property
- **CharacterAttributes**: Needs `MetaInfo` and `AttributeDefinition` navigation properties
- **CharacterBackground**: Needs `CharacterDetails` collection and single navigation
- **CharacterDetails**: Needs `MetaInfo` navigation property (FK: MetaInfoId)
- **CharacterIdentity**: Needs `Project` and identity name/navigation
- **ClassTemplate**: Needs `AttributeSet` collection
- **ClassTemplateAttribute**: Needs `ClassTemplate` and `AttributeDefinition` navigation properties
- **MetaInfo**: Needs `MetaInfoId` FK for junction table (already has Project, Tasks, etc.)
- **ContentVersionLog**: Needs `MetaInfo` navigation property (FK: MetaInfoId)
- **DialogueBranch**: Needs `Parent` self-referencing navigation (FK: ParentNodeId)
- **DialogueNode**: Already fixed - needs `Branch` navigation (FK: BranchId) ✓
- **EndingDefinition**: Needs `Project` and `Title` properties
- **EngineExportConfig**: Needs `Project` navigation property
- **EngineFieldMapping**: Needs `EngineExportConfig`, `SourceColumn`, `TargetColumn` properties
- **ExternalReference**: Needs `Parent` self-referencing navigation (FK: ParentId)
- **IdentityValue**: Needs multiple properties (ProjectTemplateId, IdentityName, Value, ProjectTemplate, etc.)
- **InventoryItem**: Needs `Project` and `Quantity` properties
- **Project**: Already fixed - has collection navigations ✓
- **ProjectIdentityDefinition**: Needs `IdentityName` and `Project` navigation property
- **ProjectToken**: Needs navigation properties (if missing)
- **ProjectTaskComments**: Needs `ProjectTaskId` FK (already has TaskId, needs ProjectTaskId for cascade)
- **ReviewStatus**: Already has MetaInfo ✓
- **StorySequence**: Already fixed - has Project, ParentSequence, ChildSequences, Beats ✓
- **Tag**: Already fixed ✓
- **TemplateAttributeSetDefinition**: Needs `Name` and `ProjectTemplate` navigation properties
- **TemplateClassTemplateDefinition**: Needs `ProjectTemplate` navigation property
- **TemplateIdentityDefinition**: Needs `IdentityName` (FK) and `ProjectTemplate` navigation
- **TemplateNarrativeStructure**: Needs `ProjectTemplate` navigation property
- **TokenUsageLog**: Needs `TokenId`, `ProjectId`, `UsedAt` properties (already fixed ✓)

### 2. Model Classes Needing Collection Navigation Properties
Each needs: `public virtual ICollection<RelatedEntity> RelatedEntities { get; set; } = new List<...>()`

- **AbilitySet**: Already has Project (single) - may need collection for abilities if applicable
- **AssetLink**: Already has MetaInfo ✓
- **CharacterAttributes**: Needs MetaInfo and AttributeDefinition collections
- **CharacterBackground**: Has `CharacterBackgrounds` - needs collection property with proper FK
- **ClassTemplate**: Needs AttributeSet collection
- **MetaInfo**: Already fixed with Tasks, Comments, VersionLogs, AssetLinks, MediaAttachments ✓
- **EndingDefinition**: May need collection of Endings if applicable

### 3. Missing Properties (Primitive Types)
Each needs explicit property declarations:

- **IdentityValue**: ProjectTemplateId (Guid), IdentityName (string), Value (string), IsRequired (bool), OrderIndex (int)
- **InventoryItem**: Quantity (decimal/long?)
- **EndingDefinition**: Title (string, max length 128 or similar)
- **Project**: Visibility property (already exists but EF config expecting different pattern?)
- **TemplateAttributeSetDefinition**: Name (string)

### 4. Configuration File Issues
These configs expect navigation properties that don't exist yet:

- `CharacterDetailsEntityTypeConfiguration` - expects `CharacterDetails` collection on model
- `CharacterBackgroundEntityTypeConfiguration` - expects `CharacterBackgrounds` collection
- Many others similar...

## Priority Order for Fixes:

### Critical (Break Build):
1. AbilitySet.Project
2. AssetLink.MetaInfo
3. CharacterAttributes.MetaInfo, AttributeDefinition
4. CharacterDetails.MetaInfo
5. ContentVersionLog.MetaInfo

### High Priority:
6. ClassTemplate.AttributeSet collection
7. ProjectIdentityDefinition.IdentityName, Project
8. TemplateAttributeSetDefinition.Name, ProjectTemplate
9. EndingDefinition.Project, Title
10. EngineExportConfig.Project, IsEnabled

### Medium Priority:
11-15. Various template and identity definitions with missing FK properties

## Quick Fix Pattern:

```csharp
// Example: AbilitySet needs Project navigation
public class AbilitySet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; } = "";
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    // MISSING - Add this:
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    [Required]
    public Guid ProjectId { get; set; }
}
```

## Fix Commands to Run Sequentially:

1. `dotnet build 2>&1 | grep "error CS1061" > /tmp/errors.txt && cat /tmp/errors.txt`
2. Parse output for model class names
3. Add missing `[ForeignKey]` navigation properties
4. Add missing collection properties where needed
5. Add missing primitive properties
6. Repeat until build succeeds

## Estimated Time: 2-3 hours of systematic fixes
