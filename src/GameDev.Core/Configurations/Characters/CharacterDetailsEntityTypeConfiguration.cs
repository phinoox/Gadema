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
        
        // Navigation property: ContentItem (Cascade delete) - Many-to-One relationship
        // CharacterDetails has ONE ContentItem, but ContentItem may have zero or many CharacterDetails
        builder.HasOne(cd => cd.ContentItem)  // ✅ Fixed: Navigate to the actual ContentItem property
            .WithMany(ci => ci.CharacterDetailsCollection)  // ⚠️ Need to add collection property to ContentItem model
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete character details when content deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();

        // Collection navigation: CharacterBackgrounds (via junction table pattern or direct FK)
        // This enables lazy loading to access all background associations for this character
        builder.HasMany(cd => cd.CharacterBackgrounds);  // Enable lazy loading
    }
}

