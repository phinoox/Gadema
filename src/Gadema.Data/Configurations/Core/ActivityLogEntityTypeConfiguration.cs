using Gadema.Core.Models.Base.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Core;

/// <summary>
/// Configuration for ActivityLog entity in activity tracking system.
/// </summary>
public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Relationship configuration for Cascade Delete
        builder.HasOne(al => al.Project)
            .WithMany() 
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
}
