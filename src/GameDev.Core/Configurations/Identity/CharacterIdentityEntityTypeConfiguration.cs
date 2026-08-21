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
        // Primary key (composite)
        builder.HasKey(e => new { e.ContentItemId, e.ProjectId, e.IdentityName });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.IdentityName);
    }
}
