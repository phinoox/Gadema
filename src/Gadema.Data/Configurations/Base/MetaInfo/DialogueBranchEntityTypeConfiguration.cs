// src/Gadema.Data/Configurations/Writing/DialogueBranchEntityTypeConfiguration.cs
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DialogueBranchEntityTypeConfiguration : IEntityTypeConfiguration<DialogueBranch>
{
    public void Configure(EntityTypeBuilder<DialogueBranch> builder)
    {
        builder.ToTable("DialogueBranches");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_DialogueBranch_MetaInfoId");
    }
}
