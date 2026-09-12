// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for MetaInfo entity in game development management system.
/// </summary>
public class MetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<MetaInfo>
{
    /// <summary>
    /// Configure MetaInfo entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<MetaInfo> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.ContentType);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.IsPublic);
        builder.HasIndex(e => e.ProjectId);
        
        
        // Navigation property: MediaAttachments (SetNull to preserve attachment history)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.MetaInfo)
            .HasForeignKey(m => m.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(mi => mi.Project).
                WithMany().
                HasForeignKey(mi => mi.ProjectId).
                OnDelete(DeleteBehavior.Cascade); 
        
        // Navigation property: MetaInfoTags (SetNull to preserve tags)
        builder.HasMany(ci => ci.MetaInfoTagRelations)
            .WithOne(ct => ct.MetaInfo)
            .HasForeignKey(ct => ct.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Navigation property: ReviewStatus (SetNull to preserve review history)
        builder.HasOne(ci => ci.ReviewStatus)
            .WithOne()
            .HasForeignKey<ReviewStatus>(rs => rs.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: Comments (SetNull to preserve comment history)
        builder.HasMany(ci => ci.Comments)
            .WithOne(c => c.MetaInfo)
            .HasForeignKey(c => c.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}
