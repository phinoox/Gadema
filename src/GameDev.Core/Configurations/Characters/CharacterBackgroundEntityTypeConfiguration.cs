// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Characters;

/// <summary>
/// Configuration for CharacterBackground entity in game development management system.
/// Uses FK as PK pattern (FK = content item ID).
/// </summary>
public class CharacterBackgroundEntityTypeConfiguration : IEntityTypeConfiguration<CharacterBackground>
{
    /// <summary>
    /// Configure CharacterBackground entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterBackground> builder)
    {
        // Primary key: FK as PK pattern (FK = content item ID)
        builder.HasKey(e => e.ContentItemId);
        
        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(cb => cb)  // Self-referencing FK navigation
            .WithMany(ci => ci.CharacterBackgrounds)
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete character background when content deleted
        
        // Properties configuration
        builder.Property(e => e.FullBiography).HasMaxLength(4096);
    }
}
