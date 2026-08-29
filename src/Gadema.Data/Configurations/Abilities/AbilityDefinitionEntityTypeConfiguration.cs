// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Abilities;

/// <summary>
/// Configuration for AbilityDefinition entity in game development management system.
/// </summary>
public class AbilityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<AbilityDefinition>
{
    /// <summary>
    /// Configure AbilityDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AbilityDefinition> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.AbilityType);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
