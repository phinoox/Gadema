// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for ProjectIdentityDefinition entity in game development management system.
/// </summary>
public class ProjectIdentityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<ProjectIdentityDefinition>
{
    /// <summary>
    /// Configure ProjectIdentityDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectIdentityDefinition> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectId, e.IdentityName });
        
        // Navigation property: Project
        builder.HasOne(pid => pid.Project)
            .WithMany()
            .HasForeignKey(pid => pid.ProjectId);
    }
}
