using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Data.Configurations.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Base.MetaInfo;

public class ContentTagRelationEntityTypeConfiguration : TagRelationEntityTypeConfiguration<ContentTagRelation, ContentMetaInfo>
{
    // No extra configuration needed unless content tags have unique rules
}