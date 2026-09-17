using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Base;
using Gadema.Core.Models.Tags;

namespace Gadema.Data.Configurations.Base.MetaInfo;

public class ContentTagRelationEntityTypeConfiguration : IEntityTypeConfiguration<ContentTagRelation>
{
    public void Configure(EntityTypeBuilder<ContentTagRelation> builder)
    {
        // Composite Primary Key: A piece of content can only have a specific tag once
        builder.HasKey(r => new { r.MetaInfoId, r.TagId });

        // Relationship to ContentMetaInfo (The Content Anchor)
        builder.HasOne(r => r.ContentMetaInfo)
            .WithMany() // ContentMetaInfo doesn't need a collection of content tags
            .HasForeignKey(r => r.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to MetaTag
        builder.HasOne(r => r.Tag)
            .WithMany() // Tag doesn't need a collection of relations back to content
            .HasForeignKey(r => r.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}