// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Configurations.Content;

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
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.ContentType);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Published);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ci => ci.Project)
            .WithMany(p => p.ContentItems)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: MediaAttachments (SetNull to preserve attachment history)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.ContentItem)
            .HasForeignKey(m => m.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Navigation property: ContentTags (SetNull to preserve tags)
        builder.HasMany(ci => ci.ContentTagAssociations)
            .WithOne(ct => ct.ContentItem)
            .HasForeignKey(ct => ct.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Navigation property: ReviewStatus (SetNull to preserve review history)
        builder.HasOne(ci => ci.ReviewStatus)
            .WithMany()
            .HasForeignKey(rs => rs.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Navigation property: Comments (SetNull to preserve comment history)
        builder.HasMany(ci => ci.Comments)
            .WithOne(c => c.ContentItem)
            .HasForeignKey(c => c.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}
