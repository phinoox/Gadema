// src/Gadema.Data/Configurations/Writing/OutlineSectionEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class OutlineSectionEntityTypeConfiguration : IEntityTypeConfiguration<OutlineSection>
{
    public void Configure(EntityTypeBuilder<OutlineSection> builder)
    {
        builder.ToTable("OutlineSections");
        builder.HasKey(e => e.Id);

        // Many-to-Many with StoryBeat
        builder.HasMany(e => e.LinkedBeats)
               .WithMany(e => e.LinkedOutlineSections)
               .UsingEntity(j => j.ToTable("OutlineSectionBeats"));

        // Indexes
        builder.HasIndex(e => e.StoryOutlineId).HasDatabaseName("IX_OutlineSection_StoryOutlineId");
    }
}