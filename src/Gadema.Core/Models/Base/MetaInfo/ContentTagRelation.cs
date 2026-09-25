namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Concrete implementation for Content $\rightarrow$ Tag relationships.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo),typeof(MetaTag))] // Tells seeder: "Seed the MetaInfo first"
public class ContentTagRelation : TagRelation<ContentMetaInfo>
{
}