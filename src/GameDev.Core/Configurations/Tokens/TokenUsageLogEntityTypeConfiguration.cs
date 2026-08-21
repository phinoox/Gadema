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
        // Primary key (composite)
        builder.HasKey(e => new { e.ProjectTokenId, e.UsageCount });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectTokenId);
        builder.HasIndex(e => e.UsedAt);
    }
}
