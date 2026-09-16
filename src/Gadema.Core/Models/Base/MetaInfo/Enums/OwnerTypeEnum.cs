namespace Gadema.Core.Models.Base.MetaInfo.Enums;

/// <summary>
/// Project owner types for polymorphic ownership pattern.
/// </summary>
public enum OwnerTypeEnum
{
    /// <summary>
    /// Individual user owns the project.
    /// </summary>
    [Display(Name = "User")]
    User = 0,

    /// <summary>
    /// Team owns the project (shared collaboration).
    /// </summary>
    [Display(Name = "Team")]
    Team = 1,
}
