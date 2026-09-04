using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Content;

namespace Gadema.Data.Configurations.Content;

public class ContentItemTagEntityTypeConfiguration : IEntityTypeConfiguration<ContentItemTag>
{
    public void Configure(EntityTypeBuilder<ContentItemTag> builder)
    {
        builder.HasKey(e => new { e.ContentItemId, e.ContentTagId });

        builder.HasIndex(e => e.ContentTagId);

        /*
        builder.HasOne(ct => ct.ContentItem)
            .WithMany(p => p.ContentItemTags) // Requires adding this collection to ContentItem.cs
            .HasForeignKey(ct => ct.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);
*/
        builder.HasOne(ct => ct.ContentTag)
            .WithMany(t => t.ContentItemTags)
            .HasForeignKey(ct => ct.ContentTagId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion of used tags
    }
}