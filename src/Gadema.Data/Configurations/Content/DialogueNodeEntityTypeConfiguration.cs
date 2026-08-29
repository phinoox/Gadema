// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for DialogueNode entity in game development management system.
/// </summary>
public class DialogueNodeEntityTypeConfiguration : IEntityTypeConfiguration<DialogueNode>
{
    /// <summary>
    /// Configure DialogueNode entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<DialogueNode> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.BranchId);
        builder.HasIndex(e => e.SpeakerId);
        
        // Navigation property: DialogueBranch (Cascade delete)
        builder.HasOne(dn => dn.Branch)
            .WithMany(db => db.Nodes)
            .HasForeignKey(dn => dn.BranchId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete nodes when branch deleted
        
        // Properties configuration
        builder.Property(e => e.NodeText).IsRequired().HasMaxLength(4096);
    }
}
