// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Authentication;

/// <summary>
/// Configuration for TeamMember entity in game development management system.
/// </summary>
public class TeamMemberEntityTypeConfiguration : IEntityTypeConfiguration<TeamMember>
{
    /// <summary>
    /// Configure TeamMember entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        
        // Foreign key constraints with cascade behavior
        builder.HasOne(tm => tm.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent orphaned team members
    }
}
