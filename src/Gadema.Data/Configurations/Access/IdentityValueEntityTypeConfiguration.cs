using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Identity;

public class IdentityValueEntityTypeConfiguration : IEntityTypeConfiguration<IdentityValue>
{
    public void Configure(EntityTypeBuilder<IdentityValue> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.IdentityDefinitionId).HasDatabaseName("IX_IdentityValue_IdentityDefinitionId");
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.Slug);

        // Relationship: Link to the ContentMetaInfo anchor (The "Soul")
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany() 
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Link back to the parent IdentityDefinition (The "Body")
        builder.HasOne(e => e.IdentityDefinition)
            .WithMany() // One definition can have many values (e.g., Race has Human, Elf, etc.)
            .HasForeignKey(e => e.IdentityDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Property constraints
        builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Slug).HasMaxLength(128);
        builder.Property(e => e.Description).HasMaxLength(4096);
    }
}