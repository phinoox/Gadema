using Gadema.Core.Utils;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// A universal tag used for categorization and discovery across the entire system.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class MetaTag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();


    private string _name = "";

    [Required, MaxLength(128)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            // Automatically sync the slug whenever the name changes
            Slug = StringSanitizer.Normalize(_name);
        }
    }


        [Required, MaxLength(128), Column("slug")]
        public string Slug { get; set; } = "";

}