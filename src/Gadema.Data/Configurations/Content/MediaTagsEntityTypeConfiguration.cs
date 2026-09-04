// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for MediaTags junction entity in game development management system.
/// </summary>
public class MediaTagsEntityTypeConfiguration : IEntityTypeConfiguration<MediaTags>
{
    /// <summary>
    /// Configure MediaTags junction entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<MediaTags> builder)
    {
        // Primary key (self-composite)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MediaAttachmentId).HasDatabaseName("IX_MediaTags_Attachment");
        builder.HasIndex(e => e.TagId).HasDatabaseName("IX_MediaTags_Tag");
    }
}
//ToDo : check foreign key