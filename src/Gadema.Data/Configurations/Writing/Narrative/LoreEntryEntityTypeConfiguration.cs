using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for LoreEntry entity in game development management system.
/// </summary>
public class LoreEntryEntityTypeConfiguration : IEntityTypeConfiguration<LoreEntry>
{
    public void Configure(EntityTypeBuilder<LoreEntry> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.LoreType);
        builder.HasIndex(e => e.MetaInfoId);

        // Navigation property: ContentMetaInfo (Cascade delete)
        builder.HasOne(le => le.ContentMetaInfo)
                .WithMany()
                .HasForeignKey(le => le.MetaInfoId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade through ContentMetaInfo (we delete manually in service)

        builder.Property(e => e.RawText).HasMaxLength(4096);
        builder.Property(e => e.LoreType).IsRequired();
    }
}
