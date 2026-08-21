// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Authentication;

/// <summary>
/// Configuration for Team entity in authentication system.
/// </summary>
public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    /// <summary>
    /// Configure Team entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.CreatedByUserId);
        
        // Navigation property: TeamMembers (Cascade delete)
        builder.HasMany(t => t.TeamMembers)
            .WithOne(tm => tm.Team)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: CreatedByUser (Restrict to preserve team history)
        builder.HasOne(t => t.CreatedByUser)
            .WithMany(u => u.Teams)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Slug).IsRequired();
    }
}