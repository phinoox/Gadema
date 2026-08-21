// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Attributes;

/// <summary>
/// Configuration for AttributeSet entity in game development management system.
/// </summary>
public class AttributeSetEntityTypeConfiguration : IEntityTypeConfiguration<AttributeSet>
{
    /// <summary>
    /// Configure AttributeSet entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AttributeSet> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.DisplayOrder);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(as => as.Project)
            .WithMany()
            .HasForeignKey(as => as.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete attribute sets when project deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
