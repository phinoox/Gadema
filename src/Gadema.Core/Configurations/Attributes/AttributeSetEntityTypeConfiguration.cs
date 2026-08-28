// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Attributes;

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
        
        builder.HasOne(att => att.Project)
            .WithMany()
            .HasForeignKey(att => att.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete attribute sets when project deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
