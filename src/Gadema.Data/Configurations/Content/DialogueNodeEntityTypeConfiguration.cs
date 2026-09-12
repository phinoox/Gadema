// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Content;

// src/Gadema.Data/Configurations/Writing/DialogueNodeEntityTypeConfiguration.cs
public class DialogueNodeEntityTypeConfiguration : IEntityTypeConfiguration<DialogueNode>
{
    public void Configure(EntityTypeBuilder<DialogueNode> builder)
    {
        builder.ToTable("DialogueNodes");

        builder.HasKey(e => e.Id);

        // Updated Speaker Relationship
        builder.HasOne(e => e.Speaker)
               .WithMany() // Adjust if Character has a collection of nodes
               .HasForeignKey(e => e.SpeakerId)
               .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_DialogueNode_MetaInfoId");
        builder.HasIndex(e => e.DialogueBranchId).HasDatabaseName("IX_DialogueNode_BranchId");
    }
}