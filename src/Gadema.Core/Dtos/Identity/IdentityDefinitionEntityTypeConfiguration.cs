using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Identity;

public class IdentityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDefinition>
{
    public void Configure(EntityTypeBuilder<IdentityDefinition> builder)
    {
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.DataType);

        // Relationship to ContentMetaInfo (The Anchor)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany() // Or .WithMany(m => m.IdentityDefinitions) if you add the collection to ContentMetaInfo
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to Project
        builder.HasOne(e => e.Project)
            .WithMany() 
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Property constraints
        builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Description).HasMaxLength(256);
        builder.Property(e => e.DataType).IsRequired();
    }
}