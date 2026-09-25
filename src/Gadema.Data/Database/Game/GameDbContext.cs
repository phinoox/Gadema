using Microsoft.EntityFrameworkCore;
using Gadema.Core.Models.Game.Abilities;
using Gadema.Core.Models.Game.Attributes;
using Gadema.Core.Models.Tasks;

namespace Gadema.Data.Database.Game;

/// <summary>
/// Manages game-specific data, including abilities, attributes, and tasks.
/// </summary>
public class GameDbContext : GademaBaseContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    // Abilities and Attributes
    public DbSet<AttributeSet> AttributeSets { get; set; }
    public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
    public DbSet<AbilitySet> AbilitySets { get; set; }
    public DbSet<AbilityDefinition> AbilityDefinitions { get; set; }
    public DbSet<StatusEffectDefinition> StatusEffectDefinitions { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations specific to the Game module
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
    }
}
