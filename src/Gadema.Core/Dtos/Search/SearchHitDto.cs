namespace Gadema.Core.Dtos.Search;

public class SearchHitDto 
{
    /// <summary>The unique identifier of the actual resource (e.g., Project Id, Character Id).</summary>
    public Guid ResourceId { get; set; } 

    /// <summary>The display name for the search result list (e.g., Title or Name).</summary>
    public string DisplayName { get; set; } = "";

    /// <summary>The URL-friendly identifier used for direct navigation.</summary>
    public string Slug { get; set; } = "";

    /// <summary>The type of resource (e.g., "Project", "Character", "Scene").</summary>
    public string ResourceType { get; set; } = "";

    /// <summary>The parent context. Null for Root Anchors (Projects), ProjectId for Content.</summary>
    public Guid? ScopeId { get; set; } 

    /// <summary>The API endpoint used to fetch the full details of this resource.</summary>
    public string ResourceLink { get; set; } = "";
}