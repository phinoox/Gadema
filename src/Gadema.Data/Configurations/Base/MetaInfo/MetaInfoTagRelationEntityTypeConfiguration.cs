using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Content;

namespace Gadema.Data.Configurations.Content;

public class MetaInfoTagRelationEntityTypeConfiguration : IEntityTypeConfiguration<ContentMetaInfoTagRelation>
{
    public void Configure(EntityTypeBuilder<ContentMetaInfoTagRelation> builder)
    {
        builder.HasKey(e => new { e.MetaInfoId, e.MetaInfoTagId });

        builder.HasIndex(e => e.MetaInfoTagId);

        /*
        builder.HasOne(ct => ct.ContentMetaInfo)
            .WithMany(p => p.MetaInfoTagRelations) // Requires adding this collection to ContentMetaInfo.cs
            .HasForeignKey(ct => ct.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);
*/
        builder.HasOne(ct => ct.MetaInfoTag)
            .WithMany(t => t.MetaInfoTagRelations)
            .HasForeignKey(ct => ct.MetaInfoTagId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of used tags
    }
}