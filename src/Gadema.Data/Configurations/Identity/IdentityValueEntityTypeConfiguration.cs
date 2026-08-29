// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for IdentityValue entity in identity system.
/// </summary>
public class IdentityValueEntityTypeConfiguration : IEntityTypeConfiguration<IdentityValue>
{
    /// <summary>
    /// Configure IdentityValue entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityValue> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.IdentityTypeId);
        builder.HasIndex(e => e.IsRequired);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(iv => iv.Project)
            .WithMany()
            .HasForeignKey(iv => iv.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}