using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for StoryOutline entity in game development management system.
/// </summary>
public class StoryOutlineEntityTypeConfiguration : IEntityTypeConfiguration<StoryOutline>
{
    /// <summary>
    /// Configure StoryOutline entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StoryOutline> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("StoryOutlines");

        // 1:1 with Story
        builder.HasOne(e => e.Story)
            .WithOne(e => e.Outline)
            .HasForeignKey<StoryOutline>(e => e.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:Many with OutlineSection
        builder.HasMany(e => e.Sections)
            .WithOne(e => e.StoryOutline)
            .HasForeignKey(e => e.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_StoryOutline_MetaInfoId");
        builder.HasIndex(e => e.StoryId).HasDatabaseName("IX_StoryOutline_StoryId");
    }
}
