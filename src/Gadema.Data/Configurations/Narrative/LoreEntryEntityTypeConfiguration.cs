// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for LoreEntry entity in game development management system.
/// </summary>
public class LoreEntryEntityTypeConfiguration : IEntityTypeConfiguration<LoreEntry>
{
    /// <summary>
    /// Configure LoreEntry entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<LoreEntry> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.LoreType);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(le => le.Project)
            .WithMany()
            .HasForeignKey(le => le.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete lore when project deleted
        
        // Properties configuration
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
    }
}
