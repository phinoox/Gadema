// ... existing imports ...

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Tasks;

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
        builder.HasIndex(e => e.TargetId).HasDatabaseName("IX_Comment_TargetId"); // Replaced MetaInfoId
        builder.HasIndex(e => e.AuthorUserId).HasDatabaseName("IX_Comment_AuthorUserId"); // Replaced CommentedByUserId
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ParentCommentId); // Added index for threaded replies
        
        // Properties configuration
        builder.Property(e => e.Text).IsRequired().HasMaxLength(4096); // Updated from CommentText to match model
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}