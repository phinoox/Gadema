// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

/// <summary>
/// Configuration for Project entity in game development management system.
/// </summary>
public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configure Project entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.Visibility);        // Filter by visibility (private/public)
        builder.HasIndex(e => e.Status);            // Filter by status (draft/in-progress)
        builder.HasIndex(e => e.OwnerId);           // Search by owner
        
        // Navigation property: SeriesProject (Restrict for historical data)
        builder.HasOne(p => p.SeriesProject)
            .WithMany()
            .HasForeignKey(p => p.SeriesProjectId)  // Optional parent project for series tracking
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Navigation property: Owner (Restrict to preserve project history)
        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Navigation property: ContentItems (Cascade delete)
        builder.HasMany(p => p.ContentItems)
            .WithOne(ci => ci.Project)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete content when project deleted
    }
}
