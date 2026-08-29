// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Attributes;

/// <summary>
/// Configuration for AttributeDefinition entity in game development management system.
/// </summary>
public class AttributeDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<AttributeDefinition>
{
    /// <summary>
    /// Configure AttributeDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.ValueType);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
