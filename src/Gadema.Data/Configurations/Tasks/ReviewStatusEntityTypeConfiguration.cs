// ... existing imports ...

using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.HasIndex(e => e.TargetId).HasDatabaseName("IX_ReviewStatus_TargetId"); // Replaced MetaInfoId
        builder.HasIndex(e => e.Status);  // Filter by status
        builder.HasIndex(e => e.ReviewedByUserId).HasDatabaseName("IX_ReviewStatus_ReviewerId"); // Fixed property name
        builder.HasIndex(e => e.ReviewedAt);  // Query recent reviews
                
        // Navigation property: Reviewer (Optional FK to User)
        builder.HasOne(rs => rs.Reviewer)
            .WithMany()
            .HasForeignKey(rs => rs.ReviewedByUserId) // Fixed property name
            .OnDelete(DeleteBehavior.Restrict);

        // Properties configuration
        builder.Property(e => e.Status).IsRequired();
    }
}