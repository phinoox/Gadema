// src/Gadema.Data/Configurations/Characters/CharacterEntityTypeConfiguration.cs
using Gadema.Core.Models.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Characters;

public class CharacterEntityTypeConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        // Index on MetaInfoId for fast lookups
        builder.HasIndex(e => e.MetaInfoId)
            .HasDatabaseName("IX_Character_MetaInfoId");

        // Index on StoryProfileId for fast lookups
        builder.HasIndex(e => e.StoryProfileId)
            .HasDatabaseName("IX_Character_StoryProfileId");

        // Restrict: if MetaInfo is deleted, cascade the delete (character loses identity)
        builder.HasOne(e => e.MetaInfo)
            .WithMany()
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: StoryProfile — SetNull if profile is deleted but character remains
        builder.HasOne(e => e.StoryProfile)
            .WithOne(sp => sp.Character)
            .HasForeignKey<CharacterStoryProfile>(e => e.Id)
            .OnDelete(DeleteBehavior.SetNull);

        // SetNull: CurrentState is optional — if the current state is deleted, just clear the reference
        builder.HasOne(e => e.CurrentState)
            .WithMany(cs => cs.Characters)
            .HasForeignKey(e => e.CurrentStateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
