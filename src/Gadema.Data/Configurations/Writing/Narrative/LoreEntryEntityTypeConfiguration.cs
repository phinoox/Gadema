// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Narrative;

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
