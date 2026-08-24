// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq; // Added for LINQ methods like Include, ToListAsync
using System.Threading.Tasks;
using GameDev.Core.Dtos.DialogueTrees;
using GameDev.Core.Models;
using GameDev.Data;
using Microsoft.EntityFrameworkCore; // Added for EF Core extension methods
using Microsoft.Extensions.Logging;
using GameDev.Core.Services;

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of dialogue service.
/// </summary>
public class DialogueService : IGademaService,  IDialogueService
{
    private readonly GameDbContext _context;
    private readonly ILogger<DialogueService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.DialogueService;

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
    public async Task<ApiResponseDto<BranchListResponseDto>> GetBranchesAsync(Guid projectId)
    {
        var branches = await _context.DialogueBranches
            .Where(b => b.ProjectId == projectId)
            .OrderBy(b => b.IsRoot)
            .ThenBy(b => b.OrderIndex)
            .ToListAsync();

        // FIX: Pass actual data to DTO, not empty constructor
        return ApiResponseDto<BranchListResponseDto>.Success(
            new BranchListResponseDto 
            { 
                Items = branches.Select(b => new BranchResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    IsRoot = b.IsRoot
                })
            });
    }

    /// <summary>
    /// Create new dialogue branch.
    /// </summary>
    public async Task<ApiResponseDto<BranchResponseDto>> CreateBranchAsync(Guid projectId, CreateBranchDto createDto)
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

        // FIX: Pass actual data to DTO
        return ApiResponseDto<BranchResponseDto>.Success(
            new BranchResponseDto 
            { 
                Id = branch.Id,
                Title = branch.Title,
                Slug = branch.Slug
            });
    }
}