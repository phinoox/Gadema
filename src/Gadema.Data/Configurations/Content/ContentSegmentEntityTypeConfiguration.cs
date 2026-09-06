// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for ContentSegment entity in game development management system.
/// Acts as an index for unique token markers (shortcodes) without storing text indices.
/// Provides robustness against text edits while enabling Hyperfocus Mode and Auto-Parse.
/// </summary>
public class ContentSegmentEntityTypeConfiguration : IEntityTypeConfiguration<ContentSegment>
{
    public void Configure(EntityTypeBuilder<ContentSegment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.SceneId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: Scene (Many-to-One)
        builder.HasOne(cs => cs.Scene)
            .WithMany(s => s.ContentSegments)
            .HasForeignKey(cs => cs.SceneId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete segments when scene deleted
        
        // Properties configuration
        builder.Property(e => e.Type).HasDefaultValue(SegmentType.Dialogue);
        builder.Property(e => e.MetadataJson).HasMaxLength(4096);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Ensure MetadataJson is valid JSON (optional validation)
        // Note: SQLite doesn't support JSON validation natively, so this is handled at the application level
    }
}
