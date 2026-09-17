using Gadema.Core.Models.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Access;

public class UserProviderLinkEntityTypeConfiguration : IEntityTypeConfiguration<UserProviderLink>
{
    public void Configure(EntityTypeBuilder<UserProviderLink> builder)
    {
        builder.ToTable("UserProviderLinks");

        builder.HasKey(x => new { x.UserId, x.Provider });

        builder.HasOne(x => x.User)
               .WithMany(u => u.ProviderLinks)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ExternalSubjectId).HasMaxLength(512);
        builder.Property(x => x.LinkedAtUtc).HasDefaultValueSql("UTCNOW()");
    }
}