// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Attributes;

/// <summary>
/// Configuration for ClassTemplateAttribute entity in game development management system.
/// </summary>
public class ClassTemplateAttributeEntityTypeConfiguration : IEntityTypeConfiguration<ClassTemplateAttribute>
{
    /// <summary>
    /// Configure ClassTemplateAttribute entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ClassTemplateAttribute> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ClassTemplateId, e.AttributeDefinitionId });
        
        // Navigation properties (both required)
        builder.HasOne(cta => cta.ClassTemplate)
            .WithMany()
            .HasForeignKey(cta => cta.ClassTemplateId);
        
        builder.HasOne(cta => cta.AttributeDefinition)
            .WithMany()
            .HasForeignKey(cta => cta.AttributeDefinitionId);
    }
}
