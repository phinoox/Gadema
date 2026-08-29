// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Projects;

/// <summary>
/// Configuration for ProjectTemplate entity in game development management system.
/// </summary>
public class ProjectTemplateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTemplate>
{
    /// <summary>
    /// Configure ProjectTemplate entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectTemplate> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.TemplateType).IsUnique();
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
    }
}
