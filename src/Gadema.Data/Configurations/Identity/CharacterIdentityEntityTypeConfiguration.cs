using Gadema.Core.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for CharacterIdentity entity in game development management system.
/// </summary>
public class CharacterIdentityEntityTypeConfiguration : IEntityTypeConfiguration<CharacterIdentity>
{
    /// <summary>
    /// Configure CharacterIdentity entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterIdentity> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Link to identity type definition
        builder.HasOne(ci => ci.IdentityDefinition)
            .WithMany()
            .HasForeignKey(ci => ci.IdentityDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Link to identity value
        builder.HasOne(ci => ci.IdentityValue)
            .WithMany()
            .HasForeignKey(ci => ci.IdentityValueId)
            .OnDelete(DeleteBehavior.SetNull); 

        // Properties configuration
        builder.Property(e => e.MetaInfoId).IsRequired();
        builder.Property(e => e.IdentityTypeId).IsRequired();
        builder.Property(e => e.IdentityValueId).IsRequired();

        builder.Property(e => e.IsPrimary).HasDefaultValue(false); 
    }
}
