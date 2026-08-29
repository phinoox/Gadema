// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Activities;

/// <summary>
/// Configuration for TokenUsageLog entity in activity tracking system.
/// </summary>
public class TokenUsageLogEntityTypeConfiguration : IEntityTypeConfiguration<TokenUsageLog>
{
    /// <summary>
    /// Configure TokenUsageLog entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TokenUsageLog> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.TokenId);
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.UsedAt);  // Query recent usage
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(tul => tul.Project)
            .WithMany()
            .HasForeignKey(tul => tul.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.UsedAt).IsRequired();
    }
}