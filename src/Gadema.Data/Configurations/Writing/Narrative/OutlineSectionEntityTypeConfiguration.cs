using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for OutlineSection entity in game development management system.
/// </summary>
public class OutlineSectionEntityTypeConfiguration : IEntityTypeConfiguration<OutlineSection>
{
    /// <summary>
    /// Configure OutlineSection entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<OutlineSection> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("OutlineSections");

        // Many-to-Many with StoryBeat
        builder.HasMany(e => e.LinkedBeats)
            .WithMany(e => e.LinkedOutlineSections)
            .UsingEntity(j => j.ToTable("OutlineSectionBeats"));

        // Indexes
        builder.HasIndex(e => e.StoryOutlineId).HasDatabaseName("IX_OutlineSection_StoryOutlineId");
    }
}
