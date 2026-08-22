// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Content;

/// <summary>
/// Configuration for ContentItem entity in game development management system.
/// </summary>
public class ContentItemEntityTypeConfiguration : IEntityTypeConfiguration<ContentItem>
{
    /// <summary>
    /// Configure ContentItem entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.ContentType);       // Filter by content type (Character, World)
        builder.HasIndex(e => e.Status);            // Filter by status (Draft, Published)
        builder.HasIndex(e => e.Published);         // Filter by published flag
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ci => ci.Project)
            .WithMany(p => p.ContentItems)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete content when project deleted
        
        // Navigation property: MediaAttachments (Cascade delete)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.ContentItem)
            .HasForeignKey(m => m.MediaAttachmentId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete attachments when content deleted
        
        // Navigation property: ContentTags (SetNull to preserve tags)
        builder.HasMany(ci => ci.ContentTagAssociations)
            .WithOne(ct => ct.ContentItem)
            .HasForeignKey(ct => ct.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // Keep tag entity alive when content deleted
        
        // Navigation property: ReviewStatus (SetNull to preserve review history)
        builder.HasOne(ci => ci.ReviewStatus)
            .WithMany()
            .HasForeignKey(rs => rs.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // Preserve review history when content updated

        // Navigation property: Comments (SetNull to preserve comment history)
        builder.HasMany(ci => ci.Comments)
            .WithOne(c => c.ContentItem)
            .HasForeignKey(c => c.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // Preserve comments when content updated

        // Navigation property: Tasks (SetNull to preserve task history)
        builder.HasMany(ci => ci.Tasks)
            .WithOne(pt => pt.ContentItem)
            .HasForeignKey(pt => pt.ProjectTaskId)  // Fixed: Use ProjectTaskId instead of ContentItemId
            .OnDelete(DeleteBehavior.SetNull);  // Preserve tasks when content deleted
    }
}

