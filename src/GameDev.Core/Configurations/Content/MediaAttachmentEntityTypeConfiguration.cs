// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Content;

/// <summary>
/// Configuration for MediaAttachment entity in game development management system.
/// </summary>
public class MediaAttachmentEntityTypeConfiguration : IEntityTypeConfiguration<MediaAttachment>
{
    /// <summary>
    /// Configure MediaAttachment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<MediaAttachment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MediaAttachmentId);
        
        // Navigation property: ContentItem (SetNull to preserve attachment history)
        builder.HasOne(m => m.ContentItem)
            .WithMany(ci => ci.MediaAttachments)
            .HasForeignKey(m => m.MediaAttachmentId)
            .OnDelete(DeleteBehavior.SetNull);  // Keep attachments when content deleted
        
        // Properties configuration
        builder.Property(e => e.FileName).IsRequired();
    }
}
