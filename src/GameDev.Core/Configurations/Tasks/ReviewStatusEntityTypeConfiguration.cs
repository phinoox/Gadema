// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Tasks;

/// <summary>
/// Configuration for ReviewStatus entity in review workflow system.
/// </summary>
public class ReviewStatusEntityTypeConfiguration : IEntityTypeConfiguration<ReviewStatus>
{
    /// <summary>
    /// Configure ReviewStatus entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ReviewStatus> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.Status);  // Filter by status
        builder.HasIndex(e => e.ReviewedByUserId);
        builder.HasIndex(e => e.ReviewedAt);  // Query recent reviews
                
        // Navigation property: Reviewer (Optional FK to User)
        builder.HasOne(rs => rs.Reviewer)
            .WithMany()
            .HasForeignKey(rs => rs.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Properties configuration
        builder.Property(e => e.Status).IsRequired();
    }
}