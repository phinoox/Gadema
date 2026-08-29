// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Characters;

/// <summary>
/// Configuration for CharacterDetails entity in game development management system.
/// Uses FK as PK pattern (FK = content item ID).
/// </summary>
public class CharacterDetailsEntityTypeConfiguration : IEntityTypeConfiguration<CharacterDetails>
{
    /// <summary>
    /// Configure CharacterDetails entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterDetails> builder)
    {
        builder.HasKey(e => e.Id);
        // Primary key: FK as PK pattern (FK = content item ID)
        builder.HasOne(e => e.ContentItem).WithOne().HasForeignKey<CharacterDetails>(e => e.ContentItemId).OnDelete(DeleteBehavior.Cascade);
                
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();

        // Collection navigation: CharacterBackgrounds (via junction table pattern or direct FK)
        // This enables lazy loading to access all background associations for this character
        builder.HasMany(cd => cd.CharacterBackgrounds);  // Enable lazy loading

        builder.HasIndex(e => e.ContentItemId).IsUnique();
    }
}

