// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Abilities;

/// <summary>
/// Configuration for StatusEffectDefinition entity in game development management system.
/// </summary>
public class StatusEffectDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<StatusEffectDefinition>
{
    /// <summary>
    /// Configure StatusEffectDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StatusEffectDefinition> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.EffectType);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
