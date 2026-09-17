using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class SceneSegmentEntityTypeConfiguration : IEntityTypeConfiguration<SceneSegment>
{
    public void Configure(EntityTypeBuilder<SceneSegment> builder)
    {
        builder.HasKey(e => e.Id);

        // Indexes for performance on common lookup columns
        builder.HasIndex(e => e.SceneId).HasDatabaseName("IX_SceneSegment_SceneId");
        builder.HasIndex(e => e.Position).HasDatabaseName("IX_SceneSegment_Position");

        // Relationships
        builder.HasOne(e => e.ContentMetaInfo)
               .WithMany() // Assuming one MetaInfo per segment, but many segments can share a type? 
               .HasForeignKey(e => e.MetaInfoId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Scene)
               .WithMany() // You may want to add ICollection<SceneSegment> to the Scene model later
               .HasForeignKey(e => e.SceneId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}