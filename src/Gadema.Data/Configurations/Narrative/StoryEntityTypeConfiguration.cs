// src/Gadema.Data/Configurations/Writing/StoryEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class StoryEntityTypeConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Stories");
        builder.HasKey(e => e.Id);

        // 1:1 Relationship with StoryOutline
        builder.HasOne(e => e.Outline)
               .WithOne(e => e.Story)
               .HasForeignKey<StoryOutline>(e => e.StoryId)
               .OnDelete(DeleteBehavior.Cascade);

        // 1:Many Relationships
        builder.HasMany(e => e.Beats)
               .WithOne(e => e.Story)
               .HasForeignKey(e => e.StoryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Chapters)
               .WithOne(e => e.Story)
               .HasForeignKey(e => e.StoryId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Story_MetaInfoId");
    }
}