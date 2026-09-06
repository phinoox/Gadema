// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for DialogueBranch entity in game development management system.
/// </summary>
public class DialogueBranchEntityTypeConfiguration : IEntityTypeConfiguration<DialogueBranch>
{
    /// <summary>
    /// Configure DialogueBranch entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<DialogueBranch> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.IsRoot);

        // Self-referencing FK for tree structure
        /*builder.HasOne(db => db.ParentNode)
            .WithMany()
            .HasForeignKey(e => e.ParentNodeId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain historical data
        */

        // Add this to map the relationship
        builder.HasMany(db => db.ChildNodes)
            .WithOne() // Assuming DialogueNode has a BackReference, otherwise use WithNone()
            .HasForeignKey(n => n.DialogueBranchId)
            .OnDelete(DeleteBehavior.Cascade);
        // Properties configuration
        builder.Property(e => e.Title).IsRequired();
    }
}
