using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Content;

namespace Gadema.Data.Configurations.Content;

public class MetaInfoTagEntityTypeConfiguration : IEntityTypeConfiguration<ContentMetaInfoTag>
{
    public void Configure(EntityTypeBuilder<ContentMetaInfoTag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
    }
}