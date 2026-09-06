using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Visibility);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.UserId); // For finding projects by creator

        // Relationships

        // CreatedByUser (Restrict to preserve history if user deleted)
        builder.HasOne(p => p.User)
            .WithMany() // User doesn't need a Projects collection necessarily, or handle it elsewhere
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProjectSeries (Optional, Restrict to preserve series if project deleted)
        builder.HasOne(p => p.ProjectSeries)
            .WithMany(ps => ps.Projects)
            .HasForeignKey(p => p.ProjectSeriesId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProjectTeams (Cascade delete: if project deleted, memberships gone)
        builder.HasMany(p => p.ProjectTeams)
            .WithOne(pt => pt.Project)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // MetaInfos (Cascade delete)
        builder.HasMany(p => p.MetaInfos)
            .WithOne(ci => ci.Project)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sequences (Cascade delete)
        builder.HasMany(p => p.Sequences)
            .WithOne(s => s.Project)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tasks (Cascade delete)
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ActivityLogs (Cascade delete)
        builder.HasMany(p => p.ActivityLogs)
            .WithOne(al => al.Project)
            .HasForeignKey(al => al.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}