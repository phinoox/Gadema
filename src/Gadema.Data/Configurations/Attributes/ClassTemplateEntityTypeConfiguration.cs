// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Attributes;

/// <summary>
/// Configuration for ClassTemplate entity in game development management system.
/// </summary>
public class ClassTemplateEntityTypeConfiguration : IEntityTypeConfiguration<ClassTemplate>
{
    /// <summary>
    /// Configure ClassTemplate entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ClassTemplate> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.AttributeSetId);
        
        // Navigation property: AttributeSet (Cascade delete)
        builder.HasOne(ct => ct.AttributeSet)
            .WithMany()
            .HasForeignKey(ct => ct.AttributeSetId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete class template when attribute set deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}
