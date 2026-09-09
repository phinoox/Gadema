// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for StoryBeat entity in game development management system.
/// A "Landmark" or major keypoint within a StoryOutline - becomes draggable "Cards" in the Right Panel.
/// Hierarchy: StoryOutline → StoryBeat ↔ Scene (Many-to-Many via junction table)
/// </summary>
public class StoryBeatEntityTypeConfiguration : IEntityTypeConfiguration<StoryBeat>
{
   public void Configure(EntityTypeBuilder<StoryBeat> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        
        builder.HasIndex(e => e.MetaInfoId);           
        builder.HasIndex(e => e.StoryOutlineId);       
        builder.HasIndex(e => e.OrderIndex);           
        
        // MetaInfo relationship (REQUIRED, not nullable)
        builder.HasOne(sb => sb.MetaInfo)
            .WithMany()
            .HasForeignKey(sb => sb.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);        
        
        // StoryOutline relationship
        builder.HasOne(sb => sb.StoryOutline)
            .WithMany(o => o.StoryBeats)
            .HasForeignKey(sb => sb.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);         
        
        // Properties (only fields that still exist on the model)
        builder.Property(e => e.Description).HasMaxLength(4096);
        builder.Property(e => e.OrderIndex).HasDefaultValue(0);
    }
}
