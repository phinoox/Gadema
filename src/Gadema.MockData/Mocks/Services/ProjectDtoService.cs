using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.MockData.Mocks.Interfaces;
using Gadema.Core.Models.Base.Projects.Enums;
using Gadema.Core.Models.Base.Enums;
using Gadema.MockData.Mocks.DataStore;
using Gadema.MockData.Utils;

namespace Gadema.MockData.Mocks.Services;

public class ProjectDtoService : IMockService<ProjectResponseDto>
{
    private static readonly IFixture _fixture = new Fixture();

    public ProjectResponseDto Create(object input)
    {
        if (input is ProjectCreateDto createDto)
        {
            var project = new ProjectResponseDto
            {
                Id = Guid.NewGuid(),
                Title = createDto.MetaInfo.Title,
                Slug = createDto.MetaInfo.Slug,
                Status = createDto.MetaInfo.Status,
                ViewMode = createDto.MetaInfo.ViewMode,
                CreatedAt = DateTime.UtcNow,
                Description = createDto.Description,
                IsActive = true,
                PrimaryFormat = createDto.PrimaryFormat,
                Genre = createDto.Genre,
                Theme = createDto.Theme,
                Tone = createDto.Tone,
                Audience = createDto.Audience
            };
            
            // Assuming MockDataStore.Projects exists based on the pattern
            MockDataStore.Projects.Add(project);
            return project;
        }

        return CreateDefault();
    }

    public ProjectResponseDto Get(Guid id) => 
        MockDataStore.Projects.FirstOrDefault(p => p.Id == id);

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
        if (item != null) MockDataStore.Projects.Remove(item);
    }

    public IEnumerable<ProjectResponseDto> GetAll() => MockDataStore.Projects;

    private ProjectResponseDto CreateDefault() => new()
    {
        Id = Guid.NewGuid(),
        Title = _fixture.Create<string>(),
        Slug = _fixture.Create<string>(),
        Status = ProjectStatusEnum.Draft,
        ViewMode = ViewModeEnum.Presentation,
        CreatedAt = DateTime.UtcNow,
        Description = _fixture.Create<string>(),
        IsActive = true,
        PrimaryFormat = PrimaryFormatEnum.Book, 
        Genre = _fixture.Create<string>(),
        Theme = _fixture.Create<string>(),
        Tone = ToneEnum.Neutral,
        Audience = AudienceEnum.AllAges
    };

    public List<ProjectResponseDto> CreateList(int count)
    {
        var list = new List<ProjectResponseDto>();
        for (int i = 0; i < count; i++)
        {
            list.Add(CreateDefault());
        }
        return list;
    }
}