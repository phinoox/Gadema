// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Templates;

/// <summary>
/// Configuration for TemplateClassTemplateDefinition entity in game development management system.
/// </summary>
public class TemplateClassTemplateDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<TemplateClassTemplateDefinition>
{
    /// <summary>
    /// Configure TemplateClassTemplateDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TemplateClassTemplateDefinition> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTemplateId, e.ClassTemplateName });
        
        // Navigation property: ProjectTemplate
        builder.HasOne(tcdt => tcdt.ProjectTemplate)
            .WithMany()
            .HasForeignKey(tcdt => tcdt.ProjectTemplateId);
    }
}
