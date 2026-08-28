// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.EngineIntegration;

/// <summary>
/// Configuration for EngineFieldMapping entity in game development management system.
/// </summary>
public class EngineFieldMappingEntityTypeConfiguration : IEntityTypeConfiguration<EngineFieldMapping>
{
    /// <summary>
    /// Configure EngineFieldMapping entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<EngineFieldMapping> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.EngineExportConfigId, e.SourceColumn, e.TargetColumn });
        
        // Navigation property: EngineExportConfig
        builder.HasOne(efm => efm.EngineExportConfig)
            .WithMany()
            .HasForeignKey(efm => efm.EngineExportConfigId);
    }
}
