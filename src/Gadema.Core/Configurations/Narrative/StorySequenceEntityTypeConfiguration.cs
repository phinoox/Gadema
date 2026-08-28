// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Core.Configurations.Narrative;

/// <summary>
/// Configuration for StorySequence entity in game development management system.
/// </summary>
public class StorySequenceEntityTypeConfiguration : IEntityTypeConfiguration<StorySequence>
{
    /// <summary>
    /// Configure StorySequence entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StorySequence> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Published);

        // Navigation property: Project (Cascade delete)
        builder.HasOne(s => s.Project)
            .WithMany(p => p.Sequences)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete sequences when project deleted

        // Navigation property: Parent Sequence (self-referencing, restrict delete)
        // Model uses FK-as-PK pattern - HasOne with nullable foreign key for parent sequence
        // ✅ NEW - Use HasOne instead of HasOptional (FK-as-PK pattern for parent sequence)
        builder.HasOne(s => s.ParentSequence)  // FK: ParentSequenceId, PK: Id
            .WithMany(p => p.ChildSequences)
            .HasForeignKey(e => e.ParentSequenceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Collection navigation: Child sequences (inverse relationship)
        builder.HasMany(s => s.ChildSequences)
            .WithOne(cs => cs.ParentSequence)
            .HasForeignKey(cs => cs.ParentSequenceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


