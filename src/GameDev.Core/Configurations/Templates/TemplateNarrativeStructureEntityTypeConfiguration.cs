// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Templates;

/// <summary>
/// Configuration for TemplateNarrativeStructure entity in game development management system.
/// </summary>
public class TemplateNarrativeStructureEntityTypeConfiguration : IEntityTypeConfiguration<TemplateNarrativeStructure>
{
    /// <summary>
    /// Configure TemplateNarrativeStructure entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TemplateNarrativeStructure> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTemplateId, e.SequenceName });
        
        // Navigation property: ProjectTemplate
        builder.HasOne(tns => tns.ProjectTemplate)
            .WithMany()
            .HasForeignKey(tns => tns.ProjectTemplateId);
    }
}
