using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Base.Infrastructure;

namespace Gadema.Data.Configurations.Game.EngineIntegration;

/// <summary>
/// Configuration for AssetLink entity in engine integration system.
/// </summary>
public class AssetLinkEntityTypeConfiguration : IEntityTypeConfiguration<AssetLink>
{
    /// <summary>
    /// Configure AssetLink entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AssetLink> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MetaInfoId);  // Filter by content item
        
        // Navigation property: ContentMetaInfo (Cascade delete)
        builder.HasOne(al => al.ContentMetaInfo)
            .WithMany(ci => ci.AssetLinks)
            .HasForeignKey(al => al.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.EnginePath).IsRequired();
    }
}
