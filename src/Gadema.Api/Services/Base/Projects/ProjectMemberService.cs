using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.Projects;

public class ProjectMemberService : CoreService
{
    public ProjectMemberService(GameDbContext db, ILogger<ProjectMemberService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<List<ProjectMemberResponseDto>>> GetMembersAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<List<ProjectMemberResponseDto>>(projectId);
        if (error != null) return error;

        var members = await _db.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .OrderBy(pm => pm.Role) // Owners/Admins first
            .Select(pm => new ProjectMemberResponseDto
            {
                Id = pm.Id,
                UserId = pm.UserId,
                UserName = pm.User.DisplayName ?? pm.User.UserName,
                Email = pm.User.Email,
                Role = pm.Role,
                JoinedAt = pm.JoinedAt
            })
            .ToListAsync();

        return ApiResponseDto<List<ProjectMemberResponseDto>>.Success(members);
    }

    public async Task<ApiResponseDto<ProjectMemberResponseDto>> AddMemberAsync(Guid projectId, AddProjectMemberDto dto)
    {
        var error = await ValidateProjectAccessAsync<ProjectMemberResponseDto>(projectId);
        if (error != null) return error;

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null) return ApiResponseDto<ProjectMemberResponseDto>.NotFound("Project not found.");

        var existing = await _db.ProjectMembers.FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == dto.UserId);
        if (existing != null) return ApiResponseDto<ProjectMemberResponseDto>.Conflict("User is already a member.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
        if (user == null) return ApiResponseDto<ProjectMemberResponseDto>.NotFound("User not found.");

        var newMember = new ProjectMember
        {
            ProjectId = projectId,
            UserId = dto.UserId,
            Role = dto.Role
        };

        _db.ProjectMembers.Add(newMember);
        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectMemberResponseDto>.Success(new ProjectMemberResponseDto
        {
            Id = newMember.Id,
            UserId = newMember.UserId,
            UserName = user.DisplayName ?? user.UserName,
            Email = user.Email,
            Role = newMember.Role,
            JoinedAt = newMember.JoinedAt
        });
    }

    public async Task<ApiResponseDto<string>> UpdateRoleAsync(Guid projectId, Guid memberId, UpdateProjectMemberRoleDto dto)
    {
        var error = await ValidateProjectAccessAsync<string>(projectId);
        if (error != null) return error;

        var member = await _db.ProjectMembers
            .Include(pm => pm.Project)
            .FirstOrDefaultAsync(pm => pm.Id == memberId && pm.ProjectId == projectId);

        if (member == null) return ApiResponseDto<string>.NotFound("Project member not found.");

        // Prevent changing Owner role
        if (member.Role == ProjectMemberRoleEnum.Owner)
            return ApiResponseDto<string>.Forbidden("Cannot change the Owner role.");

        member.Role = dto.Role;
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Role updated successfully.");
    }

    public async Task<ApiResponseDto<string>> RemoveMemberAsync(Guid projectId, Guid memberId)
    {
        var error = await ValidateProjectAccessAsync<string>(projectId);
        if (error != null) return error;

        var member = await _db.ProjectMembers
            .Include(pm => pm.Project)
            .FirstOrDefaultAsync(pm => pm.Id == memberId && pm.ProjectId == projectId);

        if (member == null) return ApiResponseDto<string>.NotFound("Project member not found.");

        // Prevent removing Owner
        if (member.Role == ProjectMemberRoleEnum.Owner)
            return ApiResponseDto<string>.Forbidden("Cannot remove the Owner.");

        _db.ProjectMembers.Remove(member);
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Member removed successfully.");
    }
}