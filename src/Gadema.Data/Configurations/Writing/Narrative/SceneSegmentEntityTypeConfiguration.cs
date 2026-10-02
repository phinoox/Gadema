using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for SceneSegment entity in game development management system.
/// </summary>
public class SceneSegmentEntityTypeConfiguration : IEntityTypeConfiguration<SceneSegment>
{
    /// <summary>
    /// Configure SceneSegment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<SceneSegment> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.SceneId).HasDatabaseName("IX_SceneSegment_SceneId");
        builder.HasIndex(e => e.Position).HasDatabaseName("IX_SceneSegment_Position");

        // Relationship: ContentMetaInfo (Cascade delete)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany() 
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Scene (Cascade delete)
        builder.HasOne(e => e.Scene)
            .WithMany() 
            .HasForeignKey(e => e.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
