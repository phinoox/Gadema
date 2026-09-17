// src/Gadema.Data/Configurations/Writing/StoryChapterEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class StoryChapterEntityTypeConfiguration : IEntityTypeConfiguration<StoryChapter>
{
    public void Configure(EntityTypeBuilder<StoryChapter> builder)
    {
        builder.ToTable("StoryChapters");

        builder.HasKey(e => e.Id);

        // One-to-Many with Scene
        builder.HasMany(e => e.Scenes)
               .WithOne(e => e.StoryChapter)
               .HasForeignKey(e => e.StoryChapterId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_StoryChapter_MetaInfoId");
        builder.HasIndex(e => e.StoryId).HasDatabaseName("IX_StoryChapter_StoryId");
    }
}