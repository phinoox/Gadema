// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for MediaAttachmentTagRelations junction entity in game development management system.
/// </summary>
public class MediaAttachmentTagRelationsEntityTypeConfiguration : IEntityTypeConfiguration<MediaAttachmentTagRelation>
{
    /// <summary>
    /// Configure MediaAttachmentTagRelations junction entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<MediaAttachmentTagRelation> builder)
    {
        // Primary key (self-composite)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MediaAttachmentId).HasDatabaseName("IX_MediaAttachmentTagRelations_Attachment");
        builder.HasIndex(e => e.TagId).HasDatabaseName("IX_MediaAttachmentTagRelations_Tag");
    }
}
//ToDo : check foreign key