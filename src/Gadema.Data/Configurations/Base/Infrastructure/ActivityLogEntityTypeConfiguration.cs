// ... existing imports ...

using Gadema.Core.Models.Base.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Activities;

/// <summary>
/// Configuration for ActivityLog entity in activity tracking system.
/// </summary>
public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    /// <summary>
    /// Configure ActivityLog entity properties and relationships.
    /// </summary>
    // ... existing code ...

    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Relationship configuration for Cascade Delete
        builder.HasOne(al => al.Project)
            .WithMany() // Project doesn't need to know about logs, but we link them here
            .HasForeignKey(al => al.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); 

        // Indexes
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Action);
        builder.HasIndex(e => e.RelatedEntityId);
        builder.HasIndex(e => e.CreatedAt);

        // Property constraints
        builder.Property(e => e.Action).IsRequired().HasMaxLength(64);
        builder.Property(e => e.RelatedEntityType).IsRequired().HasMaxLength(64);
        builder.Property(e => e.Description).HasMaxLength(1024);
    }
// ... 
}