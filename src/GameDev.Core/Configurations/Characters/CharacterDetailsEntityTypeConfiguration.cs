// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Characters;

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
        // Primary key: FK as PK pattern (FK = content item ID)
        builder.HasKey(e => e.ContentItemId);
        
        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(cd => cd)  // Self-referencing FK navigation
            .WithMany(ci => ci.CharacterDetails)
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete character details when content deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
