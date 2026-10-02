using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

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
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("DialogueBranches");

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_DialogueBranch_MetaInfoId");
    }
}
