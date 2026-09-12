// src/Gadema.Data/Configurations/Writing/StoryOutlineEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class StoryOutlineEntityTypeConfiguration : IEntityTypeConfiguration<StoryOutline>
{
    public void Configure(EntityTypeBuilder<StoryOutline> builder)
    {
        builder.ToTable("StoryOutlines");
        builder.HasKey(e => e.Id);

        // 1:1 with Story
        builder.HasOne(e => e.Story)
               .WithOne(e => e.Outline)
               .HasForeignKey<StoryOutline>(e => e.StoryId)
               .OnDelete(DeleteBehavior.Cascade);

        // 1:Many with OutlineSection
        builder.HasMany(e => e.Sections)
               .WithOne(e => e.StoryOutline) // Parent is Outline
               .HasForeignKey(e => e.StoryOutlineId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_StoryOutline_MetaInfoId");
        builder.HasIndex(e => e.StoryId).HasDatabaseName("IX_StoryOutline_StoryId");
    }
}