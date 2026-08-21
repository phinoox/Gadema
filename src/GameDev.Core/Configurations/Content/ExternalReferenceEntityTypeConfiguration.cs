// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Content;

/// <summary>
/// Configuration for ExternalReference entity in game development management system.
/// </summary>
public class ExternalReferenceEntityTypeConfiguration : IEntityTypeConfiguration<ExternalReference>
{
    /// <summary>
    /// Configure ExternalReference entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExternalReference> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ParentId);
        builder.HasIndex(e => e.Url).IsUnique();
        
        // Navigation property: ContentItem/ProjectTask/Comment (Cascade delete)
        builder.HasOne(er => er.Parent)  // Parent navigation property
            .WithMany()
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when parent removed
        
        // Properties configuration
        builder.Property(e => e.Url).IsRequired().HasMaxLength(4096);
    }
}
