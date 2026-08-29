// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Abilities;

/// <summary>
/// Configuration for AbilitySet entity in game development management system.
/// </summary>
public class AbilitySetEntityTypeConfiguration : IEntityTypeConfiguration<AbilitySet>
{
    /// <summary>
    /// Configure AbilitySet entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AbilitySet> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Type);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(abilitySet => abilitySet.Project)
            .WithMany()
            .HasForeignKey(abilitySet => abilitySet.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete ability sets when project deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
