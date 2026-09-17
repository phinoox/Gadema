using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Tags;

namespace Gadema.Data.Configurations.Tags;

public class MetaTagEntityTypeConfiguration : IEntityTypeConfiguration<MetaTag>
{
    public void Configure(EntityTypeBuilder<MetaTag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.Slug).IsUnique();
    }
}