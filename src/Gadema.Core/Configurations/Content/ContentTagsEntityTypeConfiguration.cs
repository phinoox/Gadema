// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Content;

/// <summary>
/// Configuration for ContentTags junction entity in game development management system.
/// </summary>
public class ContentTagsEntityTypeConfiguration : IEntityTypeConfiguration<ContentTags>
{
    /// <summary>
    /// Configure ContentTags junction entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ContentTags> builder)
    {
        // Primary key (self-composite)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId).HasDatabaseName("IX_ContentTags_ContentItem");
        builder.HasIndex(e => e.TagId).HasDatabaseName("IX_ContentTags_Tag");
        
        // Properties configuration
        builder.Property(e => e.OrderIndex).HasMaxLength(128);
    }
}
