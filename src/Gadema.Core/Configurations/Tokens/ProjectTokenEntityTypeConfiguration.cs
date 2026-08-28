// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Tokens;

/// <summary>
/// Configuration for ProjectToken entity in game development management system.
/// </summary>
public class ProjectTokenEntityTypeConfiguration : IEntityTypeConfiguration<ProjectToken>
{
    /// <summary>
    /// Configure ProjectToken entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectToken> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.IsActive);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(pt => pt.Project)
            .WithMany()
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete tokens when project deleted
        
        // Properties configuration
        builder.Property(e => e.TokenHash).IsRequired().HasMaxLength(512);
    }
}
