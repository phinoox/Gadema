// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Attributes;

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
        builder.HasKey(e => new { e.MetaInfoId, e.AttributeDefinitionId });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.AttributeDefinitionId);
        
        // Navigation properties (both required)
        builder.HasOne(ca => ca.MetaInfo)
            .WithMany()
            .HasForeignKey(ca => ca.MetaInfoId);
        
        builder.HasOne(ca => ca.AttributeDefinition)
            .WithMany()
            .HasForeignKey(ca => ca.AttributeDefinitionId);
    }
}
