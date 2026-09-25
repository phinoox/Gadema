using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Writing.Narrative;

public class DialogueBranchEntityTypeConfiguration : IEntityTypeConfiguration<DialogueBranch>
{
    public void Configure(EntityTypeBuilder<DialogueBranch> builder)
    {
        builder.ToTable("DialogueBranches");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_DialogueBranch_MetaInfoId");
    }
}
