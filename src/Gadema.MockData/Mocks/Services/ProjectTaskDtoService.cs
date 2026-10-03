using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Gadema.Core.Dtos.Tasks;
using Gadema.MockData.Mocks.Interfaces;
using Gadema.MockData.Mocks.DataStore;
using Gadema.MockData.Utils;

namespace Gadema.MockData.Mocks.Services;

public class ProjectTaskDtoService : IMockService<ProjectTaskResponseDto>
{
    private static readonly IFixture _fixture = new Fixture();

    public ProjectTaskResponseDto Create(object input)
    {
        var projectId = Guid.NewGuid();

        if (input is ProjectTaskCreateDto createDto)
        {
            projectId = createDto.ProjectId;
            return new ProjectTaskResponseDto
            {
                Id = Guid.NewGuid(),
                MetaInfoId = Guid.NewGuid(), 
                Title = createDto.MetaInfo.Title,
                Slug = createDto.MetaInfo.Slug,
                IsPublic = createDto.MetaInfo.IsPublic,
                CreatedAt = DateTime.UtcNow,
                ProjectId = projectId,
                AssignedToUserId = createDto.AssignedToUserId,
                DueDate = createDto.DueDate,
                CreatedByUserId = createDto.CreatedByUserId
            };
        }

        return CreateDefault(projectId);
    }

    public ProjectTaskResponseDto Get(Guid id) => 
        MockDataStore.ProjectTasks.FirstOrDefault(t => t.Id == id);

    public void Update(Guid id, object updateDto)
    {
        var existing = Get(id);
        if (existing != null)
        {
            ReflectionMapper.ApplyUpdate(existing, updateDto);
        }
    }

    public void Delete(Guid id)
    {
        var item = Get(id);
        if (item != null) MockDataStore.ProjectTasks.Remove(item);
    }

    public IEnumerable<ProjectTaskResponseDto> GetAll() => MockDataStore.ProjectTasks;

    private ProjectTaskResponseDto CreateDefault(Guid projectId) => new()
    {
        Id = Guid.NewGuid(),
        MetaInfoId = Guid.NewGuid(),
        Title = _fixture.Create<string>(),
        Slug = _fixture.Create<string>(),
        IsPublic = _fixture.Create<bool>(),
        CreatedAt = DateTime.UtcNow,
        ProjectId = projectId,
        AssignedToUserId = null,
        DueDate = _fixture.Create<DateTime>(),
        CreatedByUserId = Guid.NewGuid()
    };

    public List<ProjectTaskResponseDto> CreateList(Guid projectId, int count)
    {
        var list = new List<ProjectTaskResponseDto>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(CreateDefault(projectId));
        }
        return list;
    }
}