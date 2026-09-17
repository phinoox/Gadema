// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Inventory;

/// <summary>
/// Configuration for EndingDefinition entity in game development management system.
/// </summary>
public class EndingDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<EndingDefinition>
{
    /// <summary>
    /// Configure EndingDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<EndingDefinition> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Title);
        builder.HasIndex(e => e.ProjectId);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ed => ed.Project)
            .WithMany()
            .HasForeignKey(ed => ed.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete endings when project deleted
        
        // Properties configuration
        builder.Property(e => e.Title).IsRequired();
    }
}
