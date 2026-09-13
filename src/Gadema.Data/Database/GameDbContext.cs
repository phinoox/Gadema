// =============================================================================
// Gadema.Data - EF Core DbContext and Migrations Configuration
// =============================================================================

using Gadema.Data.Configurations;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Content;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Characters;
using Gadema.Core.Models.WorldBuilding;
using Gadema.Core.Models.Game.EngineIntegration;
using Gadema.Core.Models.Writing;

namespace Gadema.Data.Database;

/// <summary>
/// Main database context for GaDeMa application.
/// Supports SQLite (MVP) and PostgreSQL (Production) migration.
/// </summary>
public class GameDbContext : DbContext
{
    #region authentication
    /// <summary>
    /// User entity set.
    /// </summary>
    public DbSet<User> Users { get; set; }

    
    #endregion

    /// <summary>
    /// MetaInfo entity set.
    /// </summary>
    public DbSet<MetaInfo> MetaInfos { get; set; }


    /// <summary>
    /// ExternalReference entity set.
    /// </summary>
    public DbSet<ExternalReference> ExternalReferences { get; set; }

    /// <summary>
    /// MediaAttachment entity set.
    /// </summary>
    public DbSet<MediaAttachment> MediaAttachments { get; set; }

    /// <summary>
    /// Tag entity set.
    /// </summary>
    public DbSet<ProjectTagRelation> ProjectTagRelations { get; set; }

    /// <summary>
    /// MetaInfoTags junction table.
    /// </summary>
    public DbSet<ProjectTag> ProjectTags { get; set; }

    public DbSet<MetaInfoTag> MetaInfoTags { get; set; }

    public DbSet<MetaInfoTagRelation> MetaInfoTagRelations { get; set; }
    /// <summary>
    /// MediaAttachmentTagRelations junction table.
    /// </summary>
    public DbSet<MediaAttachmentTagRelation> MediaAttachmentTagRelations { get; set; }

    #region worldbuilding

    public DbSet<Faction> Factions { get; set; }

    public DbSet<WorldLocation> WorldLocations { get; set; }

    #endregion

    #region story

    public DbSet<Story> Stories { get; set; }

    /// <summary>
    /// StoryOutline entity set.
    /// </summary>
    public DbSet<StoryOutline> StoryOutlines { get; set; }

    /// <summary>
    /// DialogueBranch entity set.
    /// </summary>
    public DbSet<DialogueBranch> DialogueBranches { get; set; }

    /// <summary>
    /// DialogueNode entity set.
    /// </summary>
    public DbSet<DialogueNode> DialogueNodes { get; set; }


    public DbSet<OutlineSection> OutlineSections { get; set; }


    /// <summary>
    /// StorySequence entity set.
    /// </summary>
    public DbSet<StoryChapter> StorySequences { get; set; }

    /// <summary>
    /// StoryBeat entity set.
    /// </summary>
    public DbSet<StoryBeat> StoryBeats { get; set; }

    /// <summary>
    /// Scene entity set - The "Unit of Work" for writing.
    /// </summary>
    public DbSet<Scene> Scenes { get; set; }

    /// <summary>
    /// SceneSegment entity set - Unique token markers for shortcodes.
    /// </summary>
    public DbSet<SceneSegment> SceneSegments { get; set; }

    /// <summary>
    /// SceneStoryBeatMapping junction entity set.
    /// </summary>
    public DbSet<SceneStoryBeatMapping> SceneStoryBeatMappings { get; set; }

    /// <summary>
    /// CharacterRelation entity set - Evolving character relationships.
    /// </summary>
    public DbSet<CharacterRelation> CharacterRelations { get; set; }

    /// <summary>
    /// LoreEntry entity set.
    /// </summary>
    public DbSet<LoreEntry> LoreEntries { get; set; }

    public DbSet<CharacterState> CharacterStates { get; set; }

    /// <summary>
    /// Character glue entity set.
    /// </summary>
    public DbSet<Character> Characters { get; set; }

    /// <summary>
    /// CharacterStoryProfile entity set (static backstory/traits).
    /// </summary>
    public DbSet<CharacterStoryProfile> CharacterStoryProfiles { get; set; }

    

    #endregion

    #region abilities and attributes
    /// <summary>
    /// AttributeSet entity set.
    /// </summary>
    public DbSet<AttributeSet> AttributeSets { get; set; }

    /// <summary>
    /// AttributeDefinition entity set.
    /// </summary>
    public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }

    /// <summary>
    /// AbilitySet entity set.
    /// </summary>
    public DbSet<AbilitySet> AbilitySets { get; set; }

    /// <summary>
    /// AbilityDefinition entity set.
    /// </summary>
    public DbSet<AbilityDefinition> AbilityDefinitions { get; set; }

    /// <summary>
    /// StatusEffectDefinition entity set.
    /// </summary>
    public DbSet<StatusEffectDefinition> StatusEffectDefinitions { get; set; }

    #endregion

    #region tasks

    /// <summary>
    /// ProjectTask entity set.
    /// </summary>
    public DbSet<ProjectTask> ProjectTasks { get; set; }

    /// <summary>
    /// TaskComments entity set.
    /// </summary>
    public DbSet<ProjectTaskComment> ProjectTaskComments { get; set; }

    #endregion



    /// <summary>
    /// Comment entity set.
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// ContentVersionLog entity set.
    /// </summary>
    public DbSet<ContentVersionLog> ContentVersionLogs { get; set; }

    /// <summary>
    /// ReviewStatus entity set.
    /// </summary>
    public DbSet<ReviewStatus> ReviewStatuses { get; set; }

    /// <summary>
    /// ActivityLog entity set.
    /// </summary>
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    /// <summary>
    /// ProjectToken entity set.
    /// </summary>
    public DbSet<ProjectToken> ProjectTokens { get; set; }

    /// <summary>
    /// TokenUsageLog entity set.
    /// </summary>
    public DbSet<TokenUsageLog> TokenUsageLogs { get; set; }

    /// <summary>
    /// ContentSnapshot entity set.
    /// </summary>
    public DbSet<ContentSnapshot> ContentSnapshots { get; set; }

    /// <summary>
    /// InventoryItem entity set.
    /// </summary>
    public DbSet<InventoryItem> InventoryItems { get; set; }

    /// <summary>
    /// EndingDefinition entity set.
    /// </summary>
    public DbSet<EndingDefinition> EndingDefinitions { get; set; }

    /// <summary>
    /// AssetLink entity set.
    /// </summary>
    public DbSet<AssetLink> AssetLinks { get; set; }

    public DbSet<Project> Projects { get; set; }

    #region templates
    /// <summary>
    /// ProjectTemplate entity set.
    /// </summary>
    public DbSet<ProjectTemplate> ProjectTemplates { get; set; }

    /// <summary>
    /// TemplateAttributeSetDefinition entity set.
    /// </summary>
    public DbSet<TemplateAttributeSetDefinition> TemplateAttributeSetDefinitions { get; set; }

    /// <summary>
    /// TemplateClassTemplateDefinition entity set.
    /// </summary>
    public DbSet<TemplateClassTemplateDefinition> TemplateClassTemplateDefinitions { get; set; }

    /// <summary>
    /// TemplateIdentityDefinition entity set.
    /// </summary>
    public DbSet<TemplateIdentityDefinition> TemplateIdentityDefinitions { get; set; }

    /// <summary>
    /// TemplateNarrativeStructure entity set.
    /// </summary>
    public DbSet<TemplateNarrativeStructure> TemplateNarrativeStructures { get; set; }

    /// <summary>
    /// ProjectIdentityDefinition entity set.
    /// </summary>
    public DbSet<ProjectIdentityDefinition> ProjectIdentityDefinitions { get; set; }

    /// <summary>
    /// IdentityValue entity set.
    /// </summary>
    public DbSet<IdentityValue> IdentityValues { get; set; }

    /// <summary>
    /// CharacterIdentity entity set.
    /// </summary>
    public DbSet<CharacterIdentity> CharacterIdentities { get; set; }

    #endregion

    /// <summary>
    /// EngineExportConfig entity set.
    /// </summary>
    public DbSet<EngineExportConfig> EngineExportConfigs { get; set; }

    /// <summary>
    /// EngineFieldMapping entity set.
    /// </summary>
    public DbSet<EngineFieldMapping> EngineFieldMappings { get; set; }

    public DbSet<UserProviderLink> UserProviderLinks => Set<UserProviderLink>();

    public DbSet<ProjectMember> ProjectMembers { get; set; }
    
    public DbSet<ProjectSeries> ProjectSeries { get; set; }
    public DbSet<IdentityDefinition> IdentityDefinitions { get; set; }


    /// <summary>
    /// Database connection string name.
    /// </summary>
    private const string ConnectionStringName = "DefaultConnection";

    /// <summary>
    /// Constructor with database context options.
    /// </summary>
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Database context configuration with Fluent API and auto-discovery from configuration files.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from domain folders (auto-discovery)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Database context disposal.
    /// </summary>
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}