using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for Story entity in game development management system.
/// </summary>
public class StoryEntityTypeConfiguration : IEntityTypeConfiguration<Story>
{
    /// <summary>
    /// Configure Story entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("Stories");

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
