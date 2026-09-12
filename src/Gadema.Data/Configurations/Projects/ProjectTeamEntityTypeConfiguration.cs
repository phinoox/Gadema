using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectTeamEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTeam>
{
    public void Configure(EntityTypeBuilder<ProjectTeam> builder)
    {
        // Composite Primary Key
        builder.HasKey(e => new { e.ProjectId, e.TeamId });

        // Indexes for performance
        builder.HasIndex(e => e.TeamId);

        // Relationships
        builder.HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTeams)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Team)
            .WithMany(t => t.ProjectTeams) // Requires adding ProjectTeams to Team.cs
            .HasForeignKey(pt => pt.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties
        builder.Property(pt => pt.Role).IsRequired();
    }
}