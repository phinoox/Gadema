// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Identity;

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
        
        // Link to character (ContentItem)
        builder.HasOne(ci => ci.ContentItem)
            .WithMany(c => c.CharacterIdentities)
            .HasForeignKey(ci => ci.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Character identities deleted when content deleted
        
        // Link to identity type definition
        builder.HasOne(ci => ci.IdentityType)
            .WithMany()
            .HasForeignKey(ci => ci.IdentityTypeId)
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
