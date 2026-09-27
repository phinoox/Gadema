// =============================================================================
// CharacterIdentity - Entity for character identity assignments (race, faction, guild)
// =============================================================================

namespace Gadema.Core.Models.Identity;

/// <summary>
/// Represents a character's identity assignment, linking a content item to specific identity definitions and values.
/// Used for assigning traits such as race, faction, or guild to characters within the narrative structure.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class CharacterIdentity
{
    /// <summary>
        /// Unique identifier for this character identity assignment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the associated content meta information.
    /// </summary>
    // Navigation property for ContentMetaInfo (Many-to-One)
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }
    
    /// <summary>
    /// The ID of the identity definition this assignment relates to.
    /// </summary>
    public Guid? IdentityDefinitionId { get; set; }  // Nullable FK to IdentityDefinition

    /// <summary>
    /// Navigation property for the identity definition.
    /// </summary>
    // Navigation property for IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition? IdentityDefinition { get; set; }
    
    /// <summary>
    /// The ID of the specific identity value selected for this assignment.
    /// </summary>
    public Guid? IdentityValueId { get; set; }  // Nullable FK to IdentityValue selected

    /// <summary>
    /// Navigation property for the selected identity value.
    /// </summary>
    // Navigation property for IdentityValue (Many-to-One)
    [ForeignKey("IdentityValueId")]
    public virtual IdentityValue? IdentityValue { get; set; }
    
    /// <summary>
    /// A human-readable string representation of the identity (e.g., "Human Male").
    /// </summary>
    [MaxLength(1024)]
    public string? DisplayText { get; set; }  // e.g., "Human Male"

    /// <summary>
    /// The type of identity being assigned, represented as an integer mapping to IdentityTypeEnum.
    /// </summary>
    [EnumDataType(typeof(IdentityTypeEnum)), Required, Display(Name = "Identity Type")]
    public int IdentityType { get; set; }  // FK to IdentityDefinition.IdentityType
    
    /// <summary>
    /// The ID of the identity type definition.
    /// </summary>
    [Display(Name = "Identity Type ID")]
    public Guid? IdentityTypeId { get; set; } 
    
    /// <summary>
    /// Indicates whether this is a primary identity for the character (e.g., Race vs Alignment).
    /// </summary>
    [Display(Name = "Is Primary?")]
    public bool IsPrimary { get; set; } = false;  // e.g., for race (primary) vs alignment (secondary)
}