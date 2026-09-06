// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Tasks;

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
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.Status);  // Filter by status
        builder.HasIndex(e => e.ReviewerId);
        builder.HasIndex(e => e.ReviewedAt);  // Query recent reviews
                
        // Navigation property: Reviewer (Optional FK to User)
        builder.HasOne(rs => rs.Reviewer)
            .WithMany()
            .HasForeignKey(rs => rs.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history

        
        // Properties configuration
        builder.Property(e => e.Status).IsRequired();
    }
}