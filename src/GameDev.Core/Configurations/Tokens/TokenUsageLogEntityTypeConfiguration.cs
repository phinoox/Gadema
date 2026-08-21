// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Tokens;

/// <summary>
/// Configuration for TokenUsageLog entity in game development management system.
/// </summary>
public class TokenUsageLogEntityTypeConfiguration : IEntityTypeConfiguration<TokenUsageLog>
{
    /// <summary>
    /// Configure TokenUsageLog entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TokenUsageLog> builder)
    {
        // Primary key (composite: ProjectTokenId + Timestamp for unique usage tracking)
        builder.HasKey(e => new { e.ProjectTokenId, e.Timestamp });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectTokenId);  // Filter by project
        builder.HasIndex(e => e.Action);          // Filter by action type
        builder.HasIndex(e => e.Timestamp);       // Query recent usage

        // Navigation property: ProjectToken (Cascade delete)
        builder.HasOne(tul => tul.ProjectToken)
            .WithMany(p => p.UsageLogs)  // Lazy loading navigation
            .HasForeignKey(tul => tul.ProjectTokenId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.ProjectTokenId).IsRequired();
        builder.Property(e => e.Action).IsRequired();
    }
}

