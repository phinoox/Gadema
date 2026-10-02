using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Identity;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for IdentityDefinition entity in game development management system.
/// </summary>
public class IdentityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<IdentityDefinition>
{
    /// <summary>
    /// Configure IdentityDefinition entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<IdentityDefinition> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.ProjectId);

        // Relationship to ContentMetaInfo (The Anchor)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany() 
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to Project
        builder.HasOne(e => e.Project)
            .WithMany() 
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
