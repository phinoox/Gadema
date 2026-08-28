# Code Files Summary - GaDeMa Project

## Complete List of C# Files (93 source files)

### 📁 Gadema.Api (21 files)
**Controllers:**
- Authentication/AuthController.cs
- Content/CommentsController.cs
- Content/ContentItemsController.cs
- Content/DialogueBranchesController.cs
- Content/ExternalReferencesController.cs
- Content/ReviewStatusController.cs
- Content/SearchController.cs
- Content/TagsController.cs
- Export/ExportController.cs
- Narrative/StorySequencesController.cs
- Projects/ProjectsController.cs
- Tasks/ProjectTaskController.cs

**Services:**
- Authentication/ApiAuthService.cs
- Content/ContentItemService.cs
- Content/DialogueService.cs
- Content/ExternalReferenceService.cs
- Content/TagService.cs
- EngineIntegration/ExportService.cs
- Narrative/StoryOutlineService.cs
- Projects/ProjectService.cs
- Tasks/CommentService.cs
- Tasks/ReviewStatusService.cs
- Tasks/TaskService.cs

**Other:**
- Program.cs
- ProjectInfo.cs
- Services/GlobalConfiguration.cs
- Services/Interfaces/IApiAuthService.cs
- Services/Interfaces/IContentService.cs
- Services/Interfaces/IProjectService.cs
- Services/Interfaces/IStoryOutlineService.cs
- Services/OtherInterfaces.cs
- Services/ServiceHelpers.cs

---

### 📁 Gadema.Core (74 files)
**Models (36 files):**
- Abilities/AbilityDefinition.cs
- Abilities/AbilitySet.cs
- Abilities/StatusEffectDefinition.cs
- Activities/ActivityLog.cs
- Activities/TokenUsageLog.cs
- Attributes/AttributeDefinition.cs
- Attributes/AttributeSet.cs
- Attributes/CharacterAttributes.cs
- Attributes/ClassTemplate.cs
- Attributes/ClassTemplateAttribute.cs
- Authentication/Team.cs
- Authentication/TeamMember.cs
- Authentication/User.cs
- Characters/CharacterBackground.cs
- Characters/CharacterDetails.cs
- Comment.cs
- Content/ContentItem.cs
- Content/ContentTags.cs
- Content/DialogueBranch.cs
- Content/DialogueNode.cs
- Content/ExternalReference.cs
- Content/MediaAttachment.cs
- Content/MediaTags.cs
- Content/StoryOutline.cs
- Content/Tag.cs
- EngineIntegration/AssetLink.cs
- EngineIntegration/EngineExportConfig.cs
- EngineIntegration/EngineFieldMapping.cs
- Identity/CharacterIdentity.cs
- Identity/IdentityValue.cs
- Identity/ProjectIdentityDefinition.cs
- Inventory/EndingDefinition.cs
- Inventory/InventoryItem.cs
- Narrative/LoreEntry.cs
- Narrative/StoryBeat.cs
- Narrative/StorySequence.cs
- Projects/Project.cs
- Tasks/ProjectTask.cs
- Tasks/ProjectTaskComments.cs
- Tasks/Tasks/ReviewStatus.cs
- Templates/ProjectTemplate.cs
- Templates/TemplateAttributeSetDefinition.cs
- Templates/TemplateClassTemplateDefinition.cs
- Templates/TemplateIdentityDefinition.cs
- Templates/TemplateNarrativeStructure.cs
- Tokens/ProjectToken.cs
- Versioning/ContentSnapshot.cs
- Versioning/ContentVersionLog.cs

**Dtos (28 files):**
- Authentication/Disable2FADto.cs
- Authentication/GoogleCallbackDto.cs
- Authentication/RecoveryCodesDto.cs
- Authentication/RecoveryCodesResponseDto.cs
- Authentication/SignInDto.cs
- Authentication/SignInWith2FADto.cs
- Comments/CommentListResponseDto.cs
- Comments/CreateCommentDto.cs
- Content/ExternalReferenceCreateDto.cs
- ContentItems/AutosaveDto.cs
- ContentItems/AutosaveResponseDto.cs
- ContentItems/ContentItemResponseDto.cs
- ContentItems/CreateContentItemDto.cs
- ContentItems/MediaAttachmentResponseDto.cs
- ContentItems/RollbackDto.cs
- ContentItems/UpdateContentItemDto.cs
- ContentItems/UploadMediaDto.cs
- DialogueTrees/BranchListResponseDto.cs
- DialogueTrees/BranchResponseDto.cs
- DialogueTrees/CreateBranchDto.cs
- Export/ExportApiResponseDto.cs
- Export/ExportCsvDto.cs
- Export/ExportJsonDto.cs
- Export/ExportPdfDto.cs
- Export/ExportXmlGddDto.cs
- ExternalReferences/ReferenceListResponseDto.cs
- ExternalReferences/ReferenceResponseDto.cs
- Narrative/CreateSequenceDto.cs
- Narrative/SequenceListResponseDto.cs
- Narrative/SequenceResponseDto.cs
- Projects/CreateProjectDto.cs
- Projects/ProjectResponseDto.cs
- Projects/ProjectTokenDto.cs
- Projects/ProjectTokenResponseDto.cs
- Projects/TokenListResponseDto.cs
- Projects/UpdateProjectDto.cs
- Response/SimpleResponseDto.cs
- Reviews/ApproveContentDto.cs
- Reviews/ReviewStatusResponseDto.cs
- Search/SearchContentItemsDto.cs
- Tags/AddTagsDto.cs
- Tags/TagListResponseDto.cs
- Tasks/TaskResponseDto.cs

**Configurations (18 files):**
- Abilities/AbilityDefinitionEntityTypeConfiguration.cs
- Abilities/AbilitySetEntityTypeConfiguration.cs
- Abilities/StatusEffectDefinitionEntityTypeConfiguration.cs
- Activities/ActivityLogEntityTypeConfiguration.cs
- Activities/TokenUsageLogEntityTypeConfiguration.cs
- Attributes/AttributeDefinitionEntityTypeConfiguration.cs
- Attributes/AttributeSetEntityTypeConfiguration.cs
- Attributes/CharacterAttributesEntityTypeConfiguration.cs
- Attributes/ClassTemplateAttributeEntityTypeConfiguration.cs
- Attributes/ClassTemplateEntityTypeConfiguration.cs
- Authentication/TeamEntityTypeConfiguration.cs
- Authentication/TeamMemberEntityTypeConfiguration.cs
- Authentication/UserEntityTypeConfiguration.cs
- Characters/CharacterBackgroundEntityTypeConfiguration.cs
- Characters/CharacterDetailsEntityTypeConfiguration.cs
- Content/ContentItemEntityTypeConfiguration.cs
- Content/ContentTagsEntityTypeConfiguration.cs
- Content/DialogueBranchEntityTypeConfiguration.cs
- Content/DialogueNodeEntityTypeConfiguration.cs
- Content/ExternalReferenceEntityTypeConfiguration.cs
- Content/MediaAttachmentEntityTypeConfiguration.cs
- Content/MediaTagsEntityTypeConfiguration.cs
- Content/StoryOutlineEntityTypeConfiguration.cs
- Content/TagEntityTypeConfiguration.cs
- EngineIntegration/AssetLinkEntityTypeConfiguration.cs
- EngineIntegration/EngineExportConfigEntityTypeConfiguration.cs
- EngineIntegration/EngineFieldMappingEntityTypeConfiguration.cs
- Identity/CharacterIdentityEntityTypeConfiguration.cs
- Identity/IdentityValueEntityTypeConfiguration.cs
- Identity/ProjectIdentityDefinitionEntityTypeConfiguration.cs
- Inventory/EndingDefinitionEntityTypeConfiguration.cs
- Inventory/InventoryItemEntityTypeConfiguration.cs
- Narrative/LoreEntryEntityTypeConfiguration.cs
- Narrative/StoryBeatEntityTypeConfiguration.cs
- Narrative/StorySequenceEntityTypeConfiguration.cs
- Projects/ProjectEntityTypeConfiguration.cs
- Tasks/CommentEntityTypeConfiguration.cs
- Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs
- Tasks/ProjectTaskEntityTypeConfiguration.cs
- Tasks/ReviewStatusEntityTypeConfiguration.cs
- Tasks/TaskCommentEntityTypeConfiguration.cs
- Templates/IdentityValueEntityTypeConfiguration.cs
- Templates/ProjectTemplateEntityTypeConfiguration.cs
- Templates/TemplateAttributeSetDefinitionEntityTypeConfiguration.cs
- Templates/TemplateClassTemplateDefinitionEntityTypeConfiguration.cs
- Templates/TemplateIdentityDefinitionEntityTypeConfiguration.cs
- Templates/TemplateNarrativeStructureEntityTypeConfiguration.cs
- Tokens/ProjectTokenEntityTypeConfiguration.cs
- Tokens/TokenUsageLogEntityTypeConfiguration.cs
- Versioning/ContentSnapshotEntityTypeConfiguration.cs
- Versioning/ContentVersionLogEntityTypeConfiguration.cs

**Enums (15 files):**
- ContentStatusEnum.cs
- ContentTypeEnum.cs
- ExportFormatEnum.cs
- LoreTypeEnum.cs
- OutlineStatusEnum.cs
- OwnerTypeEnum.cs
- ProjectDifficultyEnum.cs
- ProjectTemplateTypeEnum.cs
- ProjectVisibilityEnum.cs
- RelatedEntityTypeEnum.cs
- SnapshotTypeEnum.cs
- TaskDifficultyEnum.cs
- TaskPriorityEnum.cs
- TaskStatusEnum.cs
- TeamMemberRoleEnum.cs
- ViewModeEnum.cs

---

### 📁 Gadema.Data (1 file)
- GameDbContext.cs

---

### 📁 Gadema.Tests (5 files)
- Integration/ApiIntegrationTests.cs
- Models/ModelValidationTests.cs
- Services/ContentItemServiceTests.cs
- Services/ServiceIntegrationTests.cs
- TestMetadata.cs

---

### 📁 Gadema.WebApp (6 files)
- Components/AuthenticationStateProvider.cs
- Components/PageComponents.cs
- Layouts/MainLayout.cs
- Pages/DashboardPages.cs
- Program.cs

---

## Summary by Category:

| Category | Count | Description |
|----------|-------|-------------|
| Models | 36 | Core domain entities |
| Dtos | 47 | Data transfer objects for API responses |
| Controllers | 12 | HTTP request handlers |
| Services | 15 | Business logic implementations |
| Configurations | 28+ | EF Core entity configurations |
| Enums | 16 | Type-safe enumerations |

**Total Source Files: ~93** (excluding obj/generated files)
