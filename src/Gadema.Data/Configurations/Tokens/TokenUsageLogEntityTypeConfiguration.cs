// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Tokens;

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
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.TokenId);  // Filter by project token
        builder.HasIndex(e => e.ProjectId); // Filter by project
        builder.HasIndex(e => e.UsedAt);    // Query recent usage

        // Navigation property: ProjectToken (Cascade delete)
        builder.HasOne(tul => tul.ProjectToken)
            .WithMany(p => p.UsageLogs)  // Lazy loading navigation
            .HasForeignKey(tul => tul.TokenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tul => tul.ContentItem)  // FK: ContentId, PK: Id
             .WithMany(ci => ci.UsageLogs)  // If collection exists on ContentItem
             .HasForeignKey(tul => tul.ContentId)
             .OnDelete(DeleteBehavior.SetNull);

        // Navigation property: Project
        builder.HasOne(tul => tul.Project)
            .WithMany()
            .HasForeignKey(tul => tul.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.UsedAt).IsRequired();
    }
}