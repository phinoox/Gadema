using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Core;

public class ProjectTagRelationEntityTypeConfiguration : TagRelationEntityTypeConfiguration<ProjectTagRelation, ProjectMetaInfo>
{
    public override void Configure(EntityTypeBuilder<ProjectTagRelation> builder)
    {
        base.Configure(builder);
        // You can add project-specific index or property configuration here if needed
    }
}
