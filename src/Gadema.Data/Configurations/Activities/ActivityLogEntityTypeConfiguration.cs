// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Activities;

/// <summary>
/// Configuration for ActivityLog entity in activity tracking system.
/// </summary>
public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    /// <summary>
    /// Configure ActivityLog entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.CreatedAt);  // Query recent activities
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(al => al.Project)
            .WithMany()
            .HasForeignKey(al => al.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: User (Optional FK) - Changed from HasOptional to proper navigation
        builder.HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Properties configuration
        builder.Property(e => e.EventType).IsRequired();
    }
}