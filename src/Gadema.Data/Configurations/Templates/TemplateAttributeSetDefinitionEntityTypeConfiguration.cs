// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Templates;

/// <summary>
/// Configuration for TemplateAttributeSetDefinition entity in game development management system.
/// </summary>
public class TemplateAttributeSetDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<TemplateAttributeSetDefinition>
{
    /// <summary>
    /// Configure TemplateAttributeSetDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TemplateAttributeSetDefinition> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTemplateId, e.Name });
        
        // Navigation property: ProjectTemplate
        builder.HasOne(tad => tad.ProjectTemplate)
            .WithMany()
            .HasForeignKey(tad => tad.ProjectTemplateId);
    }
}
