// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Templates;

/// <summary>
/// Configuration for TemplateIdentityDefinition entity in game development management system.
/// </summary>
public class IdentityValueEntityTypeConfiguration : IEntityTypeConfiguration<IdentityValue>
{
    /// <summary>
    /// Configure IdentityValue entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityValue> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTemplateId, e.IdentityName, e.Value });
        
        // Navigation property: ProjectTemplate
        builder.HasOne(iv => iv.ProjectTemplate)
            .WithMany()
            .HasForeignKey(iv => iv.ProjectTemplateId);
    }
}
