using Gadema.Core.Enums;
using Gadema.Core.Models.Base.Projects;
using FluentAssertions;
using Xunit;

namespace Gadema.Tests.Unit.Models;

public class ProjectTests
{
    [Fact]
    public void NewProject_ShouldHaveCorrectDefaults()
    {
        // Arrange & Act
        var project = new Project();

        // Assert
        project.IsActive.Should().BeTrue();
        project.EnableUserRegistration.Should().BeFalse();
        project.AllowManualInvites.Should().BeTrue();
        project.PrimaryFormat.Should().Be(PrimaryFormatEnum.Book); // Based on your code
        project.Tone.Should().Be(ToneEnum.Neutral);
        project.Audience.Should().Be(AudienceEnum.AllAges);
    }

    [Fact]
    public void Project_ShouldAllowUpdatingDomainMetadata()
    {
        // Arrange
        var project = new Project();

        // Act
        project.Description = "A high-fantasy epic.";
        project.Genre = "Fantasy";
        project.Theme = "Magic and Loss";
        project.Tone = ToneEnum.Dark;
        project.Audience = AudienceEnum.Adult;

        // Assert
        project.Description.Should().Be("A high-fantasy epic.");
        project.Genre.Should().Be("Fantasy");
        project.Theme.Should().Be("Magic and Loss");
        project.Tone.Should().Be(ToneEnum.Dark);
        project.Audience.Should().Be(AudienceEnum.Adult);
    }

    [Fact]
    public void Project_ShouldMaintainRelationshipIds()
    {
        // Arrange
        var project = new Project();
        var userId = Guid.NewGuid();
        var metaInfoId = Guid.NewGuid();

        // Act
        project.UserId = userId;
        project.ProjectMetaInfoId = metaInfoId;

        // Assert
        project.UserId.Should().Be(userId);
        project.ProjectMetaInfoId.Should().Be(metaInfoId);
    }
}