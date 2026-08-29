// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for CharacterIdentity entity in game development management system.
/// </summary>
public class CharacterIdentityEntityTypeConfiguration : IEntityTypeConfiguration<CharacterIdentity>
{
    /// <summary>
    /// Configure CharacterIdentity entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterIdentity> builder)
    {
        builder.HasKey(e => e.Id);

        
        // Link to identity type definition
        builder.HasOne(ci => ci.IdentityDefinition)  // Navigate to IdentityDefinition instead
            .WithMany()  // If IDentityDefinition has collection, or remove if not
            .HasForeignKey(ci => ci.IdentityDefinitionId)  // Use existing FK property
            .OnDelete(DeleteBehavior.Restrict);

        // Link to identity value
        builder.HasOne(ci => ci.IdentityValue)
            .WithMany()
            .HasForeignKey(ci => ci.IdentityValueId)
            .OnDelete(DeleteBehavior.SetNull);  // Allow multiple identity values over time

        // Properties configuration
        builder.Property(e => e.ContentItemId).IsRequired();
        builder.Property(e => e.IdentityTypeId).IsRequired();
        builder.Property(e => e.IdentityValueId).IsRequired();

        builder.Property(e => e.IsPrimary).HasDefaultValue(false);  // Primary identity flag
    }
}
