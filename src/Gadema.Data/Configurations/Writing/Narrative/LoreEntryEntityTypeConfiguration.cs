using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for LoreEntry entity in game development management system.
/// </summary>
public class LoreEntryEntityTypeConfiguration : IEntityTypeConfiguration<LoreEntry>
{
    /// <summary>
    /// Configure LoreEntry entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<LoreEntry> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.LoreType);
        builder.HasIndex(e => e.MetaInfoId);

        // Relationship: ContentMetaInfo (Cascade delete)
        builder.HasOne(le => le.ContentMetaInfo)
            .WithMany()
            .HasForeignKey(le => le.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.RawText).HasMaxLength(4096);
    }
}
