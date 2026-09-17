using Gadema.Core.Models.Game.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Game.Inventory;

public class InventoryItemEntityTypeConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(e => e.Id);

        // Properties configuration
        builder.Property(e => e.ItemName).IsRequired().HasMaxLength(128);
        builder.Property(e => e.ItemType).IsRequired();
        builder.Property(e => e.Quantity).IsRequired();
        builder.Property(e => e.Published).IsRequired();

        // Relationship: Link to ContentMetaInfo (Optional)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany()
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship: Link to Project (Required)
        builder.HasOne(e => e.Project)
            .WithMany() 
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}