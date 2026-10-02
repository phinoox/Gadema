using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

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
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.DialogueBranchId).HasDatabaseName("IX_DialogueNode_BranchId");
        builder.HasIndex(e => e.ParentNodeId).HasDatabaseName("IX_DialogueNode_ParentNodeId");
        builder.HasIndex(e => e.SpeakerId).HasDatabaseName("IX_DialogueNode_SpeakerId");

        // Relationship to DialogueBranch (Cascade delete)
        builder.HasOne(e => e.DialogueBranch)
            .WithMany(b => b.ChildNodes)
            .HasForeignKey(e => e.DialogueBranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to ParentNode (Restrict delete for tree integrity)
        builder.HasOne(e => e.ParentNode)
            .WithMany(p => p.ChildNodes)
            .HasForeignKey(e => e.ParentNodeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
