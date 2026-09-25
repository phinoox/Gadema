namespace Gadema.Core.Models.Base.MetaInfo;


/// <summary>
/// Concrete implementation for Project $\rightarrow$ Tag relationships.
/// </summary>
[ModelDependency(typeof(ProjectMetaInfo),typeof(MetaTag))] // Tells seeder: "Seed the MetaInfo first"
public class ProjectTagRelation : TagRelation<ProjectMetaInfo>
{
    // This class is now empty of logic, but it provides a concrete 
    // type for the DbContext and Seeder to target.
}