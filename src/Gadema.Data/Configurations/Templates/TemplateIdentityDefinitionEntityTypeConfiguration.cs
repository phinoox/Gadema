// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Templates;

/// <summary>
/// Configuration for TemplateIdentityDefinition entity in game development management system.
/// </summary>
public class TemplateIdentityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<TemplateIdentityDefinition>
{
    /// <summary>
    /// Configure TemplateIdentityDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TemplateIdentityDefinition> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTemplateId, e.IdentityName });
        
        // Navigation property: ProjectTemplate
        builder.HasOne(tid => tid.ProjectTemplate)
            .WithMany()
            .HasForeignKey(tid => tid.ProjectTemplateId);
    }
}
