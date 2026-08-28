// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Attributes;

/// <summary>
/// Configuration for CharacterAttributes entity in game development management system.
/// </summary>
public class CharacterAttributesEntityTypeConfiguration : IEntityTypeConfiguration<CharacterAttributes>
{
    /// <summary>
    /// Configure CharacterAttributes entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterAttributes> builder)
    {
        // Primary key (composite)
        builder.HasKey(e => new { e.ContentItemId, e.AttributeDefinitionId });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.AttributeDefinitionId);
        
        // Navigation properties (both required)
        builder.HasOne(ca => ca.ContentItem)
            .WithMany()
            .HasForeignKey(ca => ca.ContentItemId);
        
        builder.HasOne(ca => ca.AttributeDefinition)
            .WithMany()
            .HasForeignKey(ca => ca.AttributeDefinitionId);
    }
}
