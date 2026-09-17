using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class DialogueNodeEntityTypeConfiguration : IEntityTypeConfiguration<DialogueNode>
{
    public void Configure(EntityTypeBuilder<DialogueNode> builder)
    {
        builder.HasIndex(e => e.DialogueBranchId).HasDatabaseName("IX_DialogueNode_BranchId");
        builder.HasIndex(e => e.ParentNodeId).HasDatabaseName("IX_DialogueNode_ParentNodeId");
        builder.HasIndex(e => e.SpeakerId).HasDatabaseName("IX_DialogueNode_SpeakerId");

        builder.HasOne(e => e.DialogueBranch)
            .WithMany(b => b.ChildNodes)
            .HasForeignKey(e => e.DialogueBranchId)
            .OnDelete(DeleteBehavior.Cascade); // If a branch is deleted, its nodes are gone.

        builder.HasOne(e => e.ParentNode)
            .WithMany(p => p.ChildNodes)
            .HasForeignKey(e => e.ParentNodeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent circular/cascading deletion in tree structure
    }
}