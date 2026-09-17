// src/Gadema.Data/Configurations/Characters/CharacterStoryProfileEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Writing.Characters;

public class CharacterStoryProfileEntityTypeConfiguration : IEntityTypeConfiguration<CharacterStoryProfile>
{
    public void Configure(EntityTypeBuilder<CharacterStoryProfile> builder)
    {
        // One-to-one: Id is both PK and FK to Character
        builder.HasOne(e => e.Character)
            .WithOne(cd => cd.StoryProfile)
            .HasForeignKey<CharacterStoryProfile>(e => e.Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Index on CharacterId for fast lookups (in case FK as PK pattern doesn't auto-index)
        builder.HasIndex(e => e.CharacterId)
            .HasDatabaseName("IX_CharacterStoryProfile_CharacterId");
    }
}
