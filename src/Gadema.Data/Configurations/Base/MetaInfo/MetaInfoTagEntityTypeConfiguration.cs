using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Base.MetaInfo;

public class MetaInfoTagEntityTypeConfiguration : IEntityTypeConfiguration<ContentMetaInfoTag>
{
    public void Configure(EntityTypeBuilder<ContentMetaInfoTag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Name);
    }
}