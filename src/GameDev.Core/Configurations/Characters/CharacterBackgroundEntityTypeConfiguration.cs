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
        
        // Navigation property: ContentItem (Cascade delete) - Many-to-One relationship
        // CharacterBackground has ONE ContentItem, but ContentItem may have zero or many CharacterBackgrounds
        builder.HasOne(cb => cb.ContentItem)  // ✅ Fixed: Navigate to the actual ContentItem property
            .WithMany(ci => ci.CharacterBackgrounds)  // ⚠️ Need to add collection property to ContentItem model
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete character background when content deleted
        
        // Navigation property: CharacterDetails (Many-to-One relationship) - REQUIRED FK
        builder.HasOne(cb => cb.CharacterDetails)  // ✅ Fixed: Navigate to the actual CharacterDetails property
            .WithMany(cd => cd.CharacterBackgrounds)
            .HasForeignKey(e => e.ContentItemId);
    }
}

