using Gadema.Core.Models.Access;
using Gadema.Core.Models.Access.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Core;

/// <summary>
/// Configuration for User entity in game development management system.
/// </summary>
public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configure User entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();

        // Properties configuration
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Provider).HasDefaultValue(UserAuthProviderEnum.Password);

        // 1:1 relationship with UserMetaInfo sharing the same ID
        builder.HasOne(u => u.MetaInfo)
               .WithOne()
               .HasForeignKey<UserMetaInfo>(m => m.Id)
               .OnDelete(DeleteBehavior.Cascade); // If user is deleted, meta info is also deleted
    }
}
