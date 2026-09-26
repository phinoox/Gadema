using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Writing.Characters;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Core.Models.Writing.WorldBuilding;

namespace Gadema.Data.Database.Writing;

public class WritingDbContext : GademaBaseContext
{
    public WritingDbContext(DbContextOptions<WritingDbContext> options) : base(options) { }

    // Characters
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterRelation> CharacterRelations { get; set; }
    public DbSet<CharacterState> CharacterStates { get; set; }
    public DbSet<CharacterStoryProfile> CharacterStoryProfiles { get; set; }

    // Narrative/Worldbuilding
    public DbSet<Story> Stories { get; set; }
    public DbSet<StoryChapter> StoryChapters { get; set; }

    
    public DbSet<StoryOutline> StoryOutlines { get; set; }
    public DbSet<StoryBeat> StoryBeats { get; set; }
    public DbSet<Scene> Scenes { get; set; }
    public DbSet<SceneSegment> SceneSegments { get; set; }
    public DbSet<SceneStoryBeatMapping> SceneStoryBeatMappings { get; set; }
    public DbSet<DialogueNode> DialogueNodes { get; set; }
    public DbSet<DialogueBranch> DialogueBranches { get; set; }
    public DbSet<LoreEntry> LoreEntries { get; set; }
    public DbSet<OutlineSection> OutlineSections { get; set; }

    // Worldbuilding
    public DbSet<Faction> Factions { get; set; }
    public DbSet<WorldLocation> WorldLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations specific to the Writing module
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WritingDbContext).Assembly);
    }
}