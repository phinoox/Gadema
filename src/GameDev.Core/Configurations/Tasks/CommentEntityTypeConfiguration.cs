// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Tasks;

/// <summary>
/// Configuration for Comment entity in game development management system.
/// </summary>
public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    /// <summary>
    /// Configure Comment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.CommentedByUserId);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(c => c.ContentItem)
            .WithMany(ci => ci.Comments)
            .HasForeignKey(c => c.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete comments when content deleted
        
        // Properties configuration
        builder.Property(e => e.CommentText).IsRequired().HasMaxLength(4096);
    }
}
