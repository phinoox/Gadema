// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for Tag entity in game development management system.
/// </summary>
public class TagEntityTypeConfiguration : IEntityTypeConfiguration<Tag>
{
    /// <summary>
    /// Configure Tag entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
    }
}
