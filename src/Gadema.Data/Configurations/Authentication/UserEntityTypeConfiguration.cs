// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Authentication;

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
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();

        // Properties configuration
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Provider).HasConversion<int>().HasDefaultValue(0);
        // Email is already [Required] + [MaxLength(256)] — ensure unique index:
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.GoogleSubjectId).IsUnique().HasFilter("\"GoogleSubjectId\" IS NOT NULL");
    }
}
