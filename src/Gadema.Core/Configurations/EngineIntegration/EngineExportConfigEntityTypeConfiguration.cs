// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.EngineIntegration;

/// <summary>
/// Configuration for EngineExportConfig entity in game development management system.
/// </summary>
public class EngineExportConfigEntityTypeConfiguration : IEntityTypeConfiguration<EngineExportConfig>
{
    /// <summary>
    /// Configure EngineExportConfig entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<EngineExportConfig> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.ExportFormat);
        builder.HasIndex(e => e.IsEnabled);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ec => ec.Project)
            .WithMany()
            .HasForeignKey(ec => ec.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete export configs when project deleted
        
        // Properties configuration
        builder.Property(e => e.ExportFormat).IsRequired();
    }
}
