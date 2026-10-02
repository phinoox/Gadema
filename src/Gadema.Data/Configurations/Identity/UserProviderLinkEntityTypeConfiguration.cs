using Gadema.Core.Models.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Identity;

/// <summary>
/// Configuration for UserProviderLink entity.
/// </summary>
public class UserProviderLinkEntityTypeConfiguration : IEntityTypeConfiguration<UserProviderLink>
{
    /// <summary>
    /// Configure UserProviderLink entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<UserProviderLink> builder)
    {
        builder.ToTable("UserProviderLinks");

        // Law I: Vertical Unification (1:1 extension). 
        // The Body's ID is also the FK to the Soul.
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
               .WithMany(u => u.ProviderLinks)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Note: ExternalSubjectId and LinkedAtUtc were removed from the model during refactoring.
    }
}
