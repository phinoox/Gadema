// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.EngineIntegration;

/// <summary>
/// Configuration for AssetLink entity in engine integration system.
/// </summary>
public class AssetLinkEntityTypeConfiguration : IEntityTypeConfiguration<AssetLink>
{
    /// <summary>
    /// Configure AssetLink entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AssetLink> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.EngineTypeId);
        builder.HasIndex(e => e.UpdatedAt);  // Query recent updates
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(al => al.Project)
            .WithMany()
            .HasForeignKey(al => al.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.Url).IsRequired();
        builder.Property(e => e.EngineTypeId).IsRequired();
    }
}