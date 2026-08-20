// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of dialogue service.
/// </summary>
public class DialogueService : IDialogueService
{
    private readonly GameDbContext _context;
    private readonly ILogger<DialogueService> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public DialogueService(GameDbContext context, ILogger<DialogueService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List dialogue branches for project.
    /// </summary>
    public async Task<ApiResponseDto<BranchListResponse>> GetBranchesAsync(Guid projectId)
    {
        var branches = await _context.DialogueBranches
            .Where(b => b.ProjectId == projectId)
            .OrderBy(b => b.IsRoot)
            .ThenBy(b => b.OrderIndex)
            .ToListAsync();

        return ApiResponseDto.Success<BranchListResponse>(new BranchListResponse());
    }

    /// <summary>
    /// Create new dialogue branch.
    /// </summary>
    public async Task<ApiResponseDto<BranchResponse>> CreateBranchAsync(Guid projectId, CreateBranchDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var branch = new DialogueBranch
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = createDto.Title,
            Slug = createDto.Slug ?? SlugHelper.GenerateSlug(createDto.Title),
            VisualNodeImageUri = createDto.VisualNodeImageUri,
            CharacterIconUri = createDto.CharacterIconUri,
            IsRoot = true,
            ParentNodeId = null!,
            OrderIndex = 0
        };

        _context.DialogueBranches.Add(branch);
        await _context.SaveChangesAsync();

        return ApiResponseDto.Success<BranchResponse>(new BranchResponse());
    }
}