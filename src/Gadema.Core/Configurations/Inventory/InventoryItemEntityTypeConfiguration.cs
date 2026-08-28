// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Inventory;

/// <summary>
/// Configuration for InventoryItem entity in game development management system.
/// </summary>
public class InventoryItemEntityTypeConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    /// <summary>
    /// Configure InventoryItem entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.ItemType);
        builder.HasIndex(e => e.Quantity);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ii => ii.Project)
            .WithMany()
            .HasForeignKey(ii => ii.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete inventory when project deleted
        
        // Properties configuration
        builder.Property(e => e.Quantity).HasDefaultValue(0);
    }
}
