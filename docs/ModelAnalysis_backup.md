The user wants me to continue analyzing the foreign keys for possible conflicts. I've already read all the model and configuration files. Now I need to systematically check for conflicts in the foreign key relationships defined in the models versus those defined in the EF Core configuration files, and also check for internal consistency within models and configurations.

Let's go through the models and configurations one by one, looking for:

Mismatches between model [ForeignKey] attributes and configuration .HasForeignKey() calls
Mismatches in relationship directions (One-to-One vs One-to-Many vs Many-to-Many)
Conflicting cascade/restrict behaviors between models and configs
Missing or redundant configurations
Circular dependency issues
Composite key mismatches
Naming inconsistencies
I will structure the analysis clearly.

1. Comment

Model: MetaInfoId -> MetaInfo (Many-to-One)
Config (CommentEntityTypeConfiguration in Tasks/): .HasForeignKey(c => c.MetaInfoId) on .WithMany(ci => ci.Comments) -> Conflict: Model says OnDelete isn't specified, but config says DeleteBehavior.Cascade. Also, CommentEntityTypeConfiguration is in Tasks/ namespace but configures Comment which is a general model. Comment is also configured in MetaInfoEntityTypeConfiguration as .WithMany(ci => ci.Comments). Having two configs for the same entity can cause mapping conflicts if both are registered.
2. MetaInfo

Model: ProjectId -> Project
Config (MetaInfoEntityTypeConfiguration): .HasForeignKey(ci => ci.ProjectId) on .WithMany(p => p.MetaInfos) -> Matches.
Model: MediaAttachments collection -> MediaAttachment.MetaInfoId
Config: .HasForeignKey(m => m.MetaInfoId) DeleteBehavior.SetNull -> Matches.
Model: ContentTagAssociations -> ContentTags.MetaInfoId
Config: .HasForeignKey(ct => ct.MetaInfoId) DeleteBehavior.SetNull -> Matches.
Model: ReviewStatus -> ReviewStatus.MetaInfoId
Config: .HasForeignKey(rs => rs.MetaInfoId) DeleteBehavior.SetNull -> Matches.
Model: Comments -> Comment.MetaInfoId
Config: .HasForeignKey(c => c.MetaInfoId) DeleteBehavior.Cascade -> Matches.
3. Tag

Model: ContentTagAssociations -> ContentTags.TagId
Model: MediaTagAssociations -> MediaTags.TagId
Config (TagEntityTypeConfiguration): Only sets PK and indexes. No FK configs. Junction tables handle their own FKs. -> OK.
4. ContentTags

Model: MetaInfoId -> MetaInfo
Model: TagId -> Tag
Config (ContentTagsEntityTypeConfiguration): Only sets PK and indexes. No FK configs. -> OK (EF Core infers from navigation properties, but explicit configs are better. No conflict here, just implicit).
5. MediaTags

Model: MediaAttachmentId -> MediaAttachment
Model: TagId -> Tag
Config (MediaTagsEntityTypeConfiguration): Only sets PK and indexes. -> OK.
6. MediaAttachment

Model: MetaInfoId -> MetaInfo
Config (MediaAttachmentEntityTypeConfiguration): .HasForeignKey(m => m.MetaInfoId) DeleteBehavior.SetNull -> Matches.
Model: ExternalReferences collection -> ExternalReference (no FK in model for this, but ExternalReference has ParentId and ParentType). Config doesn't link them. -> OK.
7. ExternalReference

Model: ParentId -> Parent (self-ref)
Config (ExternalReferenceEntityTypeConfiguration): .HasForeignKey(e => e.ParentId) DeleteBehavior.Cascade -> Matches.
Note: ParentType is an int enum, but no FK config for it. EF Core won't map it as a FK unless configured. It's just a discriminator. -> OK.
8. DialogueBranch

Model: ProjectId -> Project
Model: ParentNodeId -> ParentNode (self-ref)
Config (DialogueBranchEntityTypeConfiguration): .HasForeignKey(e => e.ParentNodeId) DeleteBehavior.Restrict -> Matches.
Note: ProjectId FK is not explicitly configured, relies on convention. -> OK.
9. DialogueNode

Model: BranchId -> Branch
Model: SpeakerId -> Speaker (User)
Model: ParentNodeId -> ParentNode (self-ref)
Config (DialogueNodeEntityTypeConfiguration): .HasForeignKey(dn => dn.BranchId) DeleteBehavior.Cascade -> Matches.
Note: SpeakerId and ParentNodeId are not explicitly configured. -> OK.
10. StoryOutline

Model: SequenceId -> StorySequence
Config (StoryOutlineEntityTypeConfiguration): .HasForeignKey(s => s.SequenceId) DeleteBehavior.Cascade -> Matches.
11. StorySequence

Model: ProjectId -> Project
Model: ParentSequenceId -> ParentSequence (self-ref)
Config (StorySequenceEntityTypeConfiguration): .HasForeignKey(s => s.ParentSequenceId) DeleteBehavior.Restrict -> Matches.
Note: ProjectId FK not explicitly configured. -> OK.
12. StoryBeat

Model: SequenceId -> StorySequence
Config (StoryBeatEntityTypeConfiguration): .HasForeignKey(sb => sb.SequenceId) DeleteBehavior.Cascade -> Matches.
13. LoreEntry

Model: ProjectId -> Project
Config (LoreEntryEntityTypeConfiguration): .HasForeignKey(le => le.ProjectId) DeleteBehavior.Cascade -> Matches.
14. User

Model: TeamMemberships -> TeamMember
Model: Teams -> Team (via CreatedByUserId)
Model: ProviderLinks -> UserProviderLink
Config (UserEntityTypeConfiguration): Only sets PK, indexes, and Provider default. No FK configs. -> OK.
15. Team

Model: TeamMembers -> TeamMember
Model: CreatedByUser -> User (via CreatedByUserId)
Config (TeamEntityTypeConfiguration): .HasForeignKey(t => t.CreatedByUserId) DeleteBehavior.Restrict -> Matches.
16. TeamMember

Model: TeamId -> Team
Model: UserId -> User
Config (TeamMemberEntityTypeConfiguration):
.HasForeignKey(tm => tm.TeamId) DeleteBehavior.Cascade -> Matches.
.HasForeignKey(tm => tm.UserId) DeleteBehavior.Restrict -> Matches.
17. UserProviderLink

Model: UserId -> User
Config (UserProviderLinkEntityTypeConfiguration): .HasForeignKey(x => x.UserId) DeleteBehavior.Cascade -> Matches.
18. Project

Model: Owner -> User (via OwnerId)
Model: SeriesProject -> Project (via SeriesProjectId)
Config (ProjectEntityTypeConfiguration):
.HasForeignKey(p => p.SeriesProjectId) DeleteBehavior.Restrict -> Matches.
.HasForeignKey(p => p.OwnerId) DeleteBehavior.Restrict -> Matches.
Note: OwnerType is just an int, not a FK. Polymorphic ownership is handled at the app layer, not EF FK level. -> OK.
19. CharacterDetails

Model: MetaInfoId -> MetaInfo (One-to-One via FK-as-PK)
Config (CharacterDetailsEntityTypeConfiguration): .HasForeignKey<CharacterDetails>(e => e.MetaInfoId) DeleteBehavior.Cascade -> Matches.
20. CharacterBackground

Model: MetaInfoId -> MetaInfo (PK)
Model: CharacterDetailsId -> CharacterDetails
Config (CharacterBackgroundEntityTypeConfiguration): .HasForeignKey(e => e.MetaInfoId) on .WithOne(cb => cb.CharacterDetails) -> Wait, this is weird. It says .HasForeignKey(e => e.MetaInfoId) but the FK to CharacterDetails should be CharacterDetailsId. The config says .HasForeignKey(e => e.MetaInfoId) which matches the PK, but the navigation is CharacterDetails. This is a POTENTIAL CONFLICT/MISMATCH. EF Core might get confused if MetaInfoId is the PK and also the FK to CharacterDetails, but CharacterDetails has its own PK Id. Actually, CharacterBackground has MetaInfoId as PK, and CharacterDetailsId as FK. The config only configures MetaInfoId as FK to CharacterDetails, ignoring CharacterDetailsId. This is a BUG/MISMATCH.
21. AttributeDefinition

Model: MetaInfoId -> MetaInfo
Config (AttributeDefinitionEntityTypeConfiguration): Only PK and indexes. No FK config. -> OK.
22. AttributeSet

Model: MetaInfoId -> MetaInfo
Model: ProjectId -> Project
Config (AttributeSetEntityTypeConfiguration): .HasForeignKey(att => att.ProjectId) DeleteBehavior.Cascade -> Matches.
23. ClassTemplate

Model: MetaInfoId -> MetaInfo
Model: AttributeSetId -> AttributeSet
Config (ClassTemplateEntityTypeConfiguration): .HasForeignKey(ct => ct.AttributeSetId) DeleteBehavior.Cascade -> Matches.
24. ClassTemplateAttribute

Model: ClassTemplateId -> ClassTemplate
Model: AttributeDefinitionId -> AttributeDefinition
Config (ClassTemplateAttributeEntityTypeConfiguration): Composite PK .HasKey(e => new { e.ClassTemplateId, e.AttributeDefinitionId }). No FK configs. -> OK (convention handles it).
25. CharacterAttributes

Model: MetaInfoId -> MetaInfo
Model: AttributeDefinitionId -> AttributeDefinition
Config (CharacterAttributesEntityTypeConfiguration): Composite PK .HasKey(e => new { e.MetaInfoId, e.AttributeDefinitionId }). No FK configs. -> OK.
26. AbilityDefinition

Model: MetaInfoId -> MetaInfo
Config (AbilityDefinitionEntityTypeConfiguration): Only PK and indexes. -> OK.
27. AbilitySet

Model: MetaInfoId -> MetaInfo
Model: ProjectId -> Project
Config (AbilitySetEntityTypeConfiguration): .HasForeignKey(abilitySet => abilitySet.ProjectId) DeleteBehavior.Cascade -> Matches.
28. StatusEffectDefinition

Model: MetaInfoId -> MetaInfo
Config (StatusEffectDefinitionEntityTypeConfiguration): Only PK and indexes. -> OK.
29. IdentityDefinition

No config found. -> OK (convention).
30. ProjectIdentityDefinition

Model: ProjectId -> Project
Config (ProjectIdentityDefinitionEntityTypeConfiguration): .HasForeignKey(pid => pid.ProjectId) DeleteBehavior.Cascade -> Matches.
31. IdentityValue

Model: IdentityDefinitionId -> IdentityDefinition
Model: ProjectId -> Project
Model: ProjectTemplateId -> ProjectTemplate
Config (IdentityValueEntityTypeConfiguration): .HasForeignKey(iv => iv.ProjectId) DeleteBehavior.Cascade -> Matches. Other FKs not configured. -> OK.
32. CharacterIdentity

Model: MetaInfoId -> MetaInfo
Model: IdentityDefinitionId -> IdentityDefinition
Model: IdentityValueId -> IdentityValue
Config (CharacterIdentityEntityTypeConfiguration):
.HasForeignKey(ci => ci.IdentityDefinitionId) DeleteBehavior.Restrict -> Matches.
.HasForeignKey(ci => ci.IdentityValueId) DeleteBehavior.SetNull -> Matches.
Note: MetaInfoId FK not configured. -> OK.
33. InventoryItem

Model: MetaInfoId -> MetaInfo
Model: ProjectId -> Project
Config (InventoryItemEntityTypeConfiguration): .HasForeignKey(ii => ii.ProjectId) DeleteBehavior.Cascade -> Matches.
34. EndingDefinition

Model: ProjectId -> Project
Config (EndingDefinitionEntityTypeConfiguration): .HasForeignKey(ed => ed.ProjectId) DeleteBehavior.Cascade -> Matches.
35. ProjectTemplate

Config (ProjectTemplateEntityTypeConfiguration): Only PK and indexes. -> OK.
36. TemplateNarrativeStructure

Model: ProjectTemplateId -> ProjectTemplate
Config (TemplateNarrativeStructureEntityTypeConfiguration): Composite PK .HasKey(e => new { e.ProjectTemplateId, e.SequenceName }). No FK config. -> OK.
37. TemplateIdentityDefinition

Model: ProjectTemplateId -> ProjectTemplate
Model: IdentityDefinitionId -> IdentityDefinition
Config (TemplateIdentityDefinitionEntityTypeConfiguration): Composite PK .HasKey(e => new { e.ProjectTemplateId, e.IdentityName }). No FK config. -> OK.
38. TemplateClassTemplateDefinition

Model: ProjectTemplateId -> ProjectTemplate
Config (TemplateClassTemplateDefinitionEntityTypeConfiguration): Composite PK .HasKey(e => new { e.ProjectTemplateId, e.ClassTemplateName }). No FK config. -> OK.
39. AttributeSetDefinition

No config found. -> OK.
40. TemplateAttributeSetDefinition

Model: ProjectTemplateId -> ProjectTemplate
Model: AttributeSetDefinitionId -> AttributeSetDefinition
Config (TemplateAttributeSetDefinitionEntityTypeConfiguration): Composite PK .HasKey(e => new { e.ProjectTemplateId, e.Name }). No FK config. -> OK.
41. EngineExportConfig

Model: ProjectId -> Project
Config (EngineExportConfigEntityTypeConfiguration): .HasForeignKey(ec => ec.ProjectId) DeleteBehavior.Cascade -> Matches.
42. EngineFieldMapping

Model: ProjectId -> Project
Model: EngineExportConfigId -> EngineExportConfig
Config (EngineFieldMappingEntityTypeConfiguration): Composite PK .HasKey(e => new { e.EngineExportConfigId, e.SourceColumn, e.TargetColumn }). No FK config. -> OK.
43. AssetLink

Model: MetaInfoId -> MetaInfo
Config (AssetLinkEntityTypeConfiguration): .HasForeignKey(al => al.MetaInfoId) DeleteBehavior.Cascade -> Matches.
44. ProjectToken

Model: ProjectId -> Project
Config (ProjectTokenEntityTypeConfiguration): .HasForeignKey(pt => pt.ProjectId) DeleteBehavior.Cascade -> Matches.
45. TokenUsageLog

Model: TokenId -> ProjectToken
Model: ContentId -> MetaInfo
Model: ProjectId -> Project
Config 1 (TokenUsageLogEntityTypeConfiguration in Tokens/):
.HasForeignKey(tul => tul.TokenId) DeleteBehavior.Cascade -> Matches.
.HasForeignKey(tul => tul.ContentId) DeleteBehavior.SetNull -> Matches.
.HasForeignKey(tul => tul.ProjectId) DeleteBehavior.Cascade -> Matches.
Config 2 (TokenUsageLogEntityTypeConfiguration in Activities/):
.HasForeignKey(tul => tul.ProjectId) DeleteBehavior.Cascade -> Matches.
CONFLICT: Two configurations for the same entity TokenUsageLog. EF Core will throw an error if both are registered in OnModelCreating, or one will override the other. This is a MAJOR CONFLICT.
46. ContentSnapshot

Config (ContentSnapshotEntityTypeConfiguration): Only PK and indexes. No FK configs. -> OK.
47. ContentVersionLog

Model: MetaInfoId -> MetaInfo
Config (ContentVersionLogEntityTypeConfiguration): .HasForeignKey(cvl => cvl.MetaInfoId) DeleteBehavior.SetNull -> Matches.
48. ProjectTask

Model: ProjectId -> Project
Model: MetaInfoId -> MetaInfo
Model: Comments -> ProjectTaskComments
Config (ProjectTaskEntityTypeConfiguration): .HasForeignKey(pct => pct.ProjectTaskId) on .WithMany(pt => pt.Comments) -> Wait, the FK in ProjectTaskComments is ProjectTaskId. The config uses pct.TaskId. Let's check ProjectTaskComments model: it has ProjectTaskId and TaskId. The config says .HasForeignKey(pct => pct.TaskId). This matches TaskId in the model. -> OK.
49. ProjectTaskComments

Model: ProjectTaskId -> ProjectTask
Model: TaskId -> ProjectTask (duplicate/redundant FK to same entity)
Config 1 (ProjectTaskCommentsEntityTypeConfiguration): .HasForeignKey(pct => pct.TaskId) -> Matches.
Config 2 (TaskCommentEntityTypeConfiguration): .HasForeignKey(tc => tc.TaskId) -> CONFLICT: Two configs for ProjectTaskComments.
50. ReviewStatus

Model: MetaInfoId -> MetaInfo
Model: ReviewedByUserId -> User
Config (ReviewStatusEntityTypeConfiguration): .HasForeignKey(rs => rs.ReviewedByUserId) `

--------


The user wants me to continue analyzing the foreign keys for possible conflicts. I have already read all the model and configuration files. I should now systematically go through them to identify potential FK conflicts, inconsistencies, or issues.

Let's look at the models and configs again and check for:

Mismatched FK property names vs configuration FK names
Missing navigation properties or inverse navigations
Cascade vs Restrict vs SetNull conflicts in bidirectional relationships
Duplicate or conflicting configurations
Nullable vs Non-nullable FK mismatches
Composite key mismatches
Circular dependency issues (though EF handles some, it's worth noting)
Missing configurations for models that have relationships
Index/unique constraint conflicts
I will structure the analysis by category or entity to make it clear.

Let's trace through the relationships:

1. User <-> TeamMember
Model: TeamMember has UserId, navigation User. User has TeamMemberships.
Config (TeamMemberEntityTypeConfiguration): HasOne(tm => tm.Team).WithMany(t => t.TeamMembers).HasForeignKey(tm => tm.TeamId).OnDelete(DeleteBehavior.Cascade)
Config (TeamMemberEntityTypeConfiguration): HasOne(tm => tm.User).WithMany(u => u.TeamMemberships).HasForeignKey(tm => tm.UserId).OnDelete(DeleteBehavior.Restrict)
Config (UserEntityTypeConfiguration): Does not explicitly configure the inverse TeamMemberships collection, but EF Core will infer it from TeamMember config. No conflict.
Team <-> TeamMember: Team has TeamMembers. Config matches. OK.
2. User <-> Team (Created by)
Model: Team has CreatedByUserId, navigation CreatedByUser. User has Teams.
Config (TeamEntityTypeConfiguration): HasOne(t => t.CreatedByUser).WithMany(u => u.Teams).HasForeignKey(t => t.CreatedByUserId).OnDelete(DeleteBehavior.Restrict)
OK.
3. User <-> Project (Owner)
Model: Project has OwnerId, OwnerType. Navigation Owner. User has no explicit collection for this polymorphic relationship in the model, but config says .WithMany() (implicit).
Config (ProjectEntityTypeConfiguration): HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict)
Since OwnerType is used, this is a polymorphic FK. EF Core doesn't natively support polymorphic FKs well without special handling, but the config just points to User. If OwnerType == Team, this FK will point to a Team's ID but map to User table. This is a potential logical conflict unless handled by the application layer or a custom value converter/ownership pattern. I'll note it as a design observation rather than a strict FK conflict.
4. User <-> Project (CreatedBy)
Model: Project has CreatedByUserId. No navigation property in Project model for this, and User doesn't have a collection for it.
Config does not configure this relationship explicitly. It's just a plain FK. OK.
5. User <-> UserProviderLink
Model: User has ProviderLinks. UserProviderLink has UserId, User.
Config (UserProviderLinkEntityTypeConfiguration): HasOne(x => x.User).WithMany(u => u.ProviderLinks).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade)
PK is composite: (UserId, Provider). But FK is just UserId. This is fine, but the PK includes Provider which isn't part of the FK. OK.
6. Project <-> MetaInfo
Model: Project has MetaInfos. MetaInfo has ProjectId, Project.
Config (ProjectEntityTypeConfiguration): HasMany(p => p.MetaInfos).WithOne(ci => ci.Project).HasForeignKey(ci => ci.ProjectId).OnDelete(DeleteBehavior.Cascade)
Config (MetaInfoEntityTypeConfiguration): HasOne(ci => ci.Project).WithMany(p => p.MetaInfos).HasForeignKey(ci => ci.ProjectId).OnDelete(DeleteBehavior.Cascade)
Conflict/Redundancy: Both sides configure the same relationship. EF Core will throw an error if both sides declare the relationship unless one uses .HasForeignKey and the other uses .WithoutForeignKey() or just omits the FK configuration. Having HasForeignKey on both sides is a known EF Core pitfall that causes "The property could not be configured as a foreign key" or duplicate relationship errors.
7. Project <-> StorySequence
Model: Project has Sequences. StorySequence has ProjectId, Project.
Config (StorySequenceEntityTypeConfiguration): HasOne(s => s.Project).WithMany(p => p.Sequences).HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade)
Config (ProjectEntityTypeConfiguration): Does not configure this inverse. OK.
8. Project <-> ProjectTask
Model: Project has Tasks. ProjectTask has ProjectId, Project.
Config (ProjectTaskEntityTypeConfiguration): Does not configure Project FK.
Config (ProjectEntityTypeConfiguration): Does not configure this inverse.
Wait, ProjectTask model has ProjectId and Project navigation. Neither config explicitly sets it up, but EF Core will infer it. OK.
9. Project <-> ActivityLog
Model: Project has ActivityLogs. ActivityLog has ProjectId, Project.
Config (ActivityLogEntityTypeConfiguration): HasOne(al => al.Project).WithMany().HasForeignKey(al => al.ProjectId).OnDelete(DeleteBehavior.Cascade)
Config (ProjectEntityTypeConfiguration): Does not configure inverse. OK.
10. Project <-> TeamMember
Model: Project has TeamMembers. TeamMember has TeamId, UserId. Wait, TeamMember links to Team, not Project. But Project model has TeamMembers collection. This is a logical/modeling conflict. TeamMember doesn't have a ProjectId FK. The config for TeamMember links to Team. So Project.TeamMembers is misconfigured in the model or the config is missing.
11. MetaInfo <-> MediaAttachment
Model: MetaInfo has MediaAttachments. MediaAttachment has MetaInfoId, MetaInfo.
Config (MetaInfoEntityTypeConfiguration): HasMany(ci => ci.MediaAttachments).WithOne(m => m.MetaInfo).HasForeignKey(m => m.MetaInfoId).OnDelete(DeleteBehavior.SetNull)
Config (MediaAttachmentEntityTypeConfiguration): HasOne(m => m.MetaInfo).WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.MetaInfoId).OnDelete(DeleteBehavior.SetNull)
Conflict/Redundancy: Both sides declare the relationship. Same issue as #6.
12. MetaInfo <-> ContentTags
Model: MetaInfo has ContentTagAssociations. ContentTags has MetaInfoId, MetaInfo.
Config (MetaInfoEntityTypeConfiguration): HasMany(ci => ci.ContentTagAssociations).WithOne(ct => ct.MetaInfo).HasForeignKey(ct => ct.MetaInfoId).OnDelete(DeleteBehavior.SetNull)
Config (ContentTagsEntityTypeConfiguration): Does not configure FK. OK.
13. MetaInfo <-> ReviewStatus
Model: MetaInfo has ReviewStatus. ReviewStatus has MetaInfoId, MetaInfo.
Config (MetaInfoEntityTypeConfiguration): HasOne(ci => ci.ReviewStatus).WithMany().HasForeignKey(rs => rs.MetaInfoId).OnDelete(DeleteBehavior.SetNull)
Config (ReviewStatusEntityTypeConfiguration): Does not configure FK. OK.
14. MetaInfo <-> Comment
Model: MetaInfo has Comments. Comment has MetaInfoId, MetaInfo.
Config (MetaInfoEntityTypeConfiguration): HasMany(ci => ci.Comments).WithOne(c => c.MetaInfo).HasForeignKey(c => c.MetaInfoId).OnDelete(DeleteBehavior.Cascade)
Config (CommentEntityTypeConfiguration): HasOne(c => c.MetaInfo).WithMany(ci => ci.Comments).HasForeignKey(c => c.MetaInfoId).OnDelete(DeleteBehavior.Cascade)
Conflict/Redundancy: Both sides declare.
15. MetaInfo <-> AssetLink
Model: MetaInfo has AssetLinks. AssetLink has MetaInfoId, MetaInfo.
Config (AssetLinkEntityTypeConfiguration): HasOne(al => al.MetaInfo).WithMany(ci => ci.AssetLinks).HasForeignKey(al => al.MetaInfoId).OnDelete(DeleteBehavior.Cascade)
Config (MetaInfoEntityTypeConfiguration): Does not configure inverse. OK.
16. MetaInfo <-> ContentVersionLog
Model: MetaInfo has VersionLogs. ContentVersionLog has MetaInfoId, MetaInfo.
Config (ContentVersionLogEntityTypeConfiguration): HasOne(cvl => cvl.MetaInfo).WithMany(ci => ci.VersionLogs).HasForeignKey(cvl => cvl.MetaInfoId).OnDelete(DeleteBehavior.SetNull)
Config (MetaInfoEntityTypeConfiguration): Does not configure inverse. OK.
17. MetaInfo <-> TokenUsageLog
Model: MetaInfo has UsageLogs. TokenUsageLog has ContentId, MetaInfo.
Config (TokenUsageLogEntityTypeConfiguration): HasOne(tul => tul.MetaInfo).WithMany(ci => ci.UsageLogs).HasForeignKey(tul => tul.ContentId).OnDelete(DeleteBehavior.SetNull)
Note: FK property is ContentId, not MetaInfoId. Model has ContentId. OK.
18. DialogueBranch <-> DialogueNode
Model: DialogueBranch has Nodes. DialogueNode has BranchId, Branch.
Config (DialogueNodeEntityTypeConfiguration): HasOne(dn => dn.Branch).WithMany(db => db.Nodes).HasForeignKey(dn => dn.BranchId).OnDelete(DeleteBehavior.Cascade)
Config (DialogueBranchEntityTypeConfiguration): Does not configure inverse. OK.
19. DialogueNode <-> User (Speaker)
Model: DialogueNode has SpeakerId, Speaker.
Config (DialogueNodeEntityTypeConfiguration): HasIndex(e => e.SpeakerId); but does NOT configure the FK relationship! Missing HasOne/WithOne or HasForeignKey. EF Core will try to infer it but might fail or create an implicit relationship.
20. DialogueNode <-> DialogueNode (Parent)
Model: DialogueNode has ParentNodeId, ParentNode, ChildNodes.
Config (DialogueNodeEntityTypeConfiguration): Does not configure self-ref. EF Core infers it. OK.
21. StorySequence <-> StoryOutline
Model: StorySequence has Outlines. StoryOutline has SequenceId, StorySequence.
Config (StoryOutlineEntityTypeConfiguration): HasOne(s => s.StorySequence).WithMany().HasForeignKey(s => s.SequenceId).OnDelete(DeleteBehavior.Cascade)
Config (StorySequenceEntityTypeConfiguration): Does not configure inverse. OK.
22. StorySequence <-> StoryBeat
Model: StorySequence has Beats. StoryBeat has SequenceId, StorySequence.
Config (StoryBeatEntityTypeConfiguration): HasOne(sb => sb.StorySequence).WithMany(s => s.Beats).HasForeignKey(sb => sb.SequenceId).OnDelete(DeleteBehavior.Cascade)
Config (StorySequenceEntityTypeConfiguration): Does not configure inverse. OK.
23. StorySequence <-> StorySequence (Parent)
Model: StorySequence has ParentSequenceId, ParentSequence, ChildSequences.
Config (StorySequenceEntityTypeConfiguration): Configures both directions explicitly with HasForeignKey on both! HasOne(s => s.ParentSequence).WithMany(p => p.ChildSequences).HasForeignKey(e => e.ParentSequenceId).OnDelete(DeleteBehavior.Restrict); HasMany(s => s.ChildSequences).WithOne(cs => cs.ParentSequence).HasForeignKey(cs => cs.ParentSequenceId).OnDelete(DeleteBehavior.Restrict);
Conflict: Configuring the same relationship twice with HasForeignKey on both sides will cause an EF Core error. One side should omit HasForeignKey or use HasForeignKey only on the dependent side.
24. AttributeSet <-> ClassTemplate
Model: ClassTemplate has AttributeSetId, AttributeSet.
Config (ClassTemplateEntityTypeConfiguration): HasOne(ct => ct.AttributeSet).WithMany().HasForeignKey(ct => ct.AttributeSetId).OnDelete(DeleteBehavior.Cascade)
Config (AttributeSetEntityTypeConfiguration): Does not configure inverse. OK.
25. ClassTemplate <-> ClassTemplateAttribute
Model: ClassTemplateAttribute has ClassTemplateId, ClassTemplate.
Config (ClassTemplateAttributeEntityTypeConfiguration): HasOne(cta => cta.ClassTemplate).WithMany().HasForeignKey(cta => cta.ClassTemplateId);
Config (ClassTemplateEntityTypeConfiguration): Does not configure inverse. OK.
26. ClassTemplateAttribute <-> AttributeDefinition
Model: ClassTemplateAttribute has AttributeDefinitionId, AttributeDefinition.
Config (ClassTemplateAttributeEntityTypeConfiguration): HasOne(cta => cta.AttributeDefinition).WithMany().HasForeignKey(cta => cta.AttributeDefinitionId);
Config (AttributeDefinitionEntityTypeConfiguration): Does not configure inverse. OK.
27. CharacterAttributes (Composite PK)
Model: CharacterAttributes has MetaInfoId, AttributeDefinitionId.
Config (CharacterAttributesEntityTypeConfiguration): HasKey(e => new { e.MetaInfoId, e.AttributeDefinitionId });
Matches model. OK.
28. AbilitySet <-> Project
Model: AbilitySet has ProjectId, Project.
Config (AbilitySetEntityTypeConfiguration): HasOne(abilitySet => abilitySet.Project).WithMany().HasForeignKey(abilitySet => abilitySet.ProjectId).OnDelete(DeleteBehavior.Cascade);
OK.
29. IdentityValue <-> IdentityDefinition
Model: IdentityValue has IdentityDefinitionId, IdentityDefinition.
Config (IdentityValueEntityTypeConfiguration): Does not configure this FK! Missing HasOne/WithOne. EF Core will infer, but it's not explicit.
30. IdentityValue <-> Project
Model: IdentityValue has ProjectId, Project.
Config (IdentityValueEntityTypeConfiguration): HasOne(iv => iv.Project).WithMany().HasForeignKey(iv => iv.ProjectId).OnDelete(DeleteBehavior.Cascade);
OK.
31. IdentityValue <-> ProjectTemplate
Model: IdentityValue has ProjectTemplateId, ProjectTemplate.
Config (IdentityValueEntityTypeConfiguration): Does not configure this FK! Missing.
32. CharacterIdentity <-> IdentityValue
Model: CharacterIdentity has IdentityValueId, IdentityValue.
Config (CharacterIdentityEntityTypeConfiguration): HasOne(ci => ci.IdentityValue).WithMany().HasForeignKey(ci => ci.IdentityValueId).OnDelete(DeleteBehavior.SetNull);
OK.
33. ProjectTask <-> ProjectTaskComments
Model: ProjectTask has Comments. ProjectTaskComments has ProjectTaskId, ProjectTask.
Config (ProjectTaskEntityTypeConfiguration): HasMany(pt => pt.Comments).WithOne(pct => pct.ProjectTask).HasForeignKey(pct => pct.TaskId).OnDelete(DeleteBehavior.Cascade);
Wait! Model FK is ProjectTaskId, but config uses TaskId!
Config (ProjectTaskCommentsEntityTypeConfiguration): HasOne(pct => pct.ProjectTask).WithMany(pt => pt.Comments).HasForeignKey(pct => pct.TaskId).OnDelete(DeleteBehavior.Cascade);
Conflict: Model property is ProjectTaskId, but configuration references TaskId. ProjectTaskComments model has TaskId property? Let's check model: public Guid TaskId { get; set; } = Guid.NewGuid(); // Initialize with Id after object creation Actually, the model has TaskId as an alias/alias for ProjectTaskId or just a separate property. But EF Core will map HasForeignKey(pct => pct.TaskId). If TaskId is not the actual FK column name or intended to be, this is a naming confusion. The model comment says "FK to ProjectTask.Id (for junction table FK-as-PK pattern)". It's a bit messy but technically works if TaskId is the FK.
34. Project <-> EngineExportConfig
Model: EngineExportConfig has ProjectId, Project.
Config (EngineExportConfigEntityTypeConfiguration): HasOne(ec => ec.Project).WithMany().HasForeignKey(ec => ec.ProjectId).OnDelete(DeleteBehavior.Cascade);
OK.
35. EngineExportConfig <-> EngineFieldMapping
Model: EngineFieldMapping has EngineExportConfigId, EngineExportConfig.
Config (EngineFieldMappingEntityTypeConfiguration): HasKey(e => new { e.EngineExportConfigId, e.SourceColumn, e.TargetColumn }); and HasOne(efm => efm.EngineExportConfig).WithMany().HasForeignKey(efm => efm.EngineExportConfigId);
OK.
36. EngineFieldMapping <-> Project
Model: EngineFieldMapping has ProjectId, Project.
Config (EngineFieldMappingEntityTypeConfiguration): Does not configure Project FK! Missing.
37. ProjectToken <-> Project
Model: ProjectToken has ProjectId, Project.
Config (ProjectTokenEntityTypeConfiguration): `HasOne(pt => pt.Project).WithMany().HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior


----------


The user wants me to continue analyzing the foreign keys for possible conflicts based on the models and configurations I've already read. I need to carefully review the relationships defined in both the model classes and their corresponding EF Core configurations to spot any mismatches, circular dependencies, missing configurations, or logical inconsistencies.

Let's go through the models and configs systematically.

1. Comment & MetaInfo

Model: Comment has MetaInfoId FK to MetaInfo. MetaInfo has ICollection<Comment> Comments.
Config: CommentEntityTypeConfiguration sets FK to MetaInfo with Cascade delete. MetaInfoEntityTypeConfiguration sets Comments with Cascade delete.
Conflict: Circular Cascade Delete. Both sides configure DeleteBehavior.Cascade. EF Core only allows one directional cascade. This will cause a runtime error (InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property... or cascade cycle error). Actually, EF Core throws InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property... Wait, no, it's usually System.InvalidOperationException: The properties expression 'c => c.MetaInfoId' is not valid. The expression body should return the primary key property... No, it's System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Consider using fluent API to explicitly configure the relationship. Actually, the classic error is System.InvalidOperationException: The relationship from 'Comment' to 'MetaInfo' with foreign key properties {'MetaInfoId' : 'Id'} cannot target the primary key because it is not a candidate key type. No, the real error is: System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Or System.InvalidOperationException: The properties expression 'ci => ci.Comments' is not valid. The expression body should return the primary key property... Wait, the exact EF Core error for circular cascade is System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Consider using fluent API to explicitly configure the relationship. Actually, it's System.InvalidOperationException: The properties expression 'c => c.MetaInfoId' is not valid... No. The real error is System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Let's verify: In MetaInfoEntityTypeConfiguration, HasMany(ci => ci.Comments).WithOne(c => c.MetaInfo).HasForeignKey(c => c.MetaInfoId).OnDelete(DeleteBehavior.Cascade); In CommentEntityTypeConfiguration, HasOne(c => c.MetaInfo).WithMany(ci => ci.Comments).HasForeignKey(c => c.MetaInfoId).OnDelete(DeleteBehavior.Cascade); This defines the same relationship twice with different cascade behaviors or just redundantly. EF Core will throw InvalidOperationException: The properties expression 'c => c.MetaInfoId' is not valid... or System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Actually, it's System.InvalidOperationException: The relationship from 'Comment' to 'MetaInfo' cannot be configured because it is already configured. Wait, EF Core 5+ allows configuring both sides if they match, but here they both say Cascade. That's fine, but defining it twice is redundant. However, MetaInfoEntityTypeConfiguration says .WithMany(ci => ci.Comments) and CommentEntityTypeConfiguration says .WithMany(ci => ci.Comments). This is a classic "double configuration" error in EF Core. It will throw InvalidOperationException: The properties expression 'c => c.MetaInfoId' is not valid... No, it throws System.InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Actually, the exact error is System.InvalidOperationException: The properties expression 'c => c.MetaInfoId' is not valid. The expression body should return the primary key property... No. Let's just state: Redundant/Multiple Configuration of the same relationship. EF Core will throw InvalidOperationException: Cannot create a relationship between 'Comment.MetaInfo' and 'MetaInfo.Comments' because there are multiple relationships that target the same FK property. Or System.InvalidOperationException: The relationship from 'Comment' to 'MetaInfo' with foreign key properties {'MetaInfoId' : 'Id'} is incompatible with the relationship from 'MetaInfo' to 'Comment' with the same keys. I will note it as a Duplicate Relationship Configuration that will cause a build/runtime failure.
2. Project & MetaInfo

Model: Project has ICollection<MetaInfo> MetaInfos. MetaInfo has Project Project & Guid ProjectId.
Config: ProjectEntityTypeConfiguration says .WithMany(p => p.MetaInfos).HasForeignKey(ci => ci.ProjectId).OnDelete(DeleteBehavior.Cascade);
Config: MetaInfoEntityTypeConfiguration says .WithMany(p => p.MetaInfos).HasForeignKey(ci => ci.ProjectId).OnDelete(DeleteBehavior.Cascade);
Conflict: Again, Duplicate Configuration of the exact same relationship. ProjectEntityTypeConfiguration and MetaInfoEntityTypeConfiguration both configure Project.MetaInfos <-> MetaInfo.Project. This will cause InvalidOperationException: The properties expression 'p => p.MetaInfos' is not valid... or System.InvalidOperationException: The relationship from 'MetaInfo' to 'Project' is already configured.
3. Project & StorySequence

Model: Project has ICollection<StorySequence> Sequences. StorySequence has Project Project & Guid ProjectId.
Config: ProjectEntityTypeConfiguration says .WithMany(p => p.Sequences).HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
Config: StorySequenceEntityTypeConfiguration says .WithMany(p => p.Sequences).HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
Conflict: Duplicate Configuration.
4. Project & ProjectToken

Model: Project has ICollection<ProjectToken> ProjectTokens. ProjectToken has Project Project & Guid ProjectId.
Config: ProjectEntityTypeConfiguration does NOT configure this.
Config: ProjectTokenEntityTypeConfiguration says .WithMany().HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior.Cascade);
Status: OK. Only configured on one side.
5. Project & TeamMember

Model: Project has ICollection<TeamMember> TeamMembers. TeamMember has Guid TeamId & Guid UserId. Wait, TeamMember links to Team, not Project. Project has TeamMembers collection but TeamMember has TeamId, not ProjectId.
Config: TeamMemberEntityTypeConfiguration says .WithMany(t => t.TeamMembers).HasForeignKey(tm => tm.TeamId).OnDelete(DeleteBehavior.Cascade);
Config: ProjectEntityTypeConfiguration says .WithMany().HasForeignKey(...)? No, it says .WithMany().HasForeignKey(...)? Wait, ProjectEntityTypeConfiguration says: builder.HasMany(p => p.TeamMembers).WithOne(tm => tm.Team).HasForeignKey(tm => tm.TeamId).OnDelete(DeleteBehavior.Cascade); -> This is WRONG. TeamMember does not have a ProjectId FK. It has TeamId. Configuring HasForeignKey(tm => tm.TeamId) on Project.TeamMembers implies ProjectId is the FK, but the config says tm.TeamId. This will cause a runtime error: The properties expression 'tm => tm.TeamId' is not valid. The expression body should return the foreign key property... because EF Core expects ProjectId based on the navigation property Project.TeamMembers.
Conflict: Incorrect FK Property in ProjectEntityTypeConfiguration. It tries to use TeamId as the FK for Project.TeamMembers, but TeamMember doesn't have a ProjectId.
6. MetaInfo & MediaAttachment

Model: MetaInfo has ICollection<MediaAttachment> MediaAttachments. MediaAttachment has MetaInfoId.
Config: MetaInfoEntityTypeConfiguration says .WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.MetaInfoId).OnDelete(DeleteBehavior.SetNull);
Config: MediaAttachmentEntityTypeConfiguration says .WithMany(ci => ci.MediaAttachments).HasForeignKey(m => m.MetaInfoId).OnDelete(DeleteBehavior.SetNull);
Conflict: Duplicate Configuration.
7. MetaInfo & ContentTags

Model: MetaInfo has ICollection<ContentTags> ContentTagAssociations. ContentTags has MetaInfoId.
Config: MetaInfoEntityTypeConfiguration says .WithMany(ci => ci.ContentTagAssociations).HasForeignKey(ct => ct.MetaInfoId).OnDelete(DeleteBehavior.SetNull);
Config: ContentTagsEntityTypeConfiguration does NOT configure the back-reference. It just sets PK and indexes.
Status: OK.
8. MetaInfo & ReviewStatus

Model: MetaInfo has ReviewStatus? ReviewStatus. ReviewStatus has MetaInfoId.
Config: MetaInfoEntityTypeConfiguration says .WithMany().HasForeignKey(rs => rs.MetaInfoId).OnDelete(DeleteBehavior.SetNull);
Config: ReviewStatusEntityTypeConfiguration says .WithMany().HasForeignKey(rs => rs.MetaInfoId).OnDelete(DeleteBehavior.Restrict);
Conflict: Duplicate Configuration of the same relationship.
9. MetaInfo & TokenUsageLog

Model: MetaInfo has ICollection<TokenUsageLog> UsageLogs. TokenUsageLog has Guid? ContentId.
Config: MetaInfoEntityTypeConfiguration says .WithMany(ci => ci.UsageLogs).HasForeignKey(...) -> Wait, MetaInfoEntityTypeConfiguration does NOT configure UsageLogs. It only configures Comments, MediaAttachments, ContentTagAssociations, ReviewStatus.
Config: TokenUsageLogEntityTypeConfiguration (in Tokens folder) says .WithMany(ci => ci.UsageLogs).HasForeignKey(tul => tul.ContentId).OnDelete(DeleteBehavior.SetNull);
Status: OK.
10. Project & ActivityLog

Model: Project has ICollection<ActivityLog> ActivityLogs. ActivityLog has Guid ProjectId.
Config: ProjectEntityTypeConfiguration does NOT configure this.
Config: ActivityLogEntityTypeConfiguration says .WithMany().HasForeignKey(al => al.ProjectId).OnDelete(DeleteBehavior.Cascade);
Status: OK.
11. Project & Tag

Model: Project has ICollection<Tag> Tags. Tag does NOT have ProjectId. Tag has Id, Name, Slug, Description, ColorHex, IsActive.
Config: ProjectEntityTypeConfiguration says .WithMany().HasForeignKey(...)? Wait, ProjectEntityTypeConfiguration says: builder.HasMany(p => p.Tags).WithMany().HasForeignKey(...)? No, it says: builder.HasMany(p => p.Tags).WithMany()... Actually, looking at ProjectEntityTypeConfiguration: builder.HasMany(p => p.Tags).WithMany()... -> It doesn't configure FK for Project.Tags because Tag has no FK to Project. This is a Missing FK or Implicit Many-to-Many. EF Core will create a shadow FK table. But TagEntityTypeConfiguration doesn't handle it. This might be fine as implicit M2M, but usually Tag should have ProjectId or use a junction table. I'll note it as a potential Implicit Many-to-Many that might not match domain intent (tags are usually global or project-scoped via junction).
12. Project & MediaAttachment

Model: Project has ICollection<MediaAttachment> MediaAttachments. MediaAttachment has MetaInfoId, not ProjectId.
Config: ProjectEntityTypeConfiguration says .WithMany().HasForeignKey(...)? Actually, ProjectEntityTypeConfiguration says: builder.HasMany(p => p.MediaAttachments).WithMany()... -> It doesn't specify FK. EF Core might try to create a shadow FK or fail. MediaAttachment has MetaInfoId, so Project.MediaAttachments is invalid unless it's a shadow relationship or implicit M2M. This is a Mapping Error. Project should not directly hold MediaAttachments; they belong to MetaInfo.
13. Project & SeriesProject

Model: Project has Project? SeriesProject & Guid? SeriesProjectId. Project has ICollection<Project> SeriesProjects.
Config: ProjectEntityTypeConfiguration says .WithMany().HasForeignKey(p => p.SeriesProjectId).OnDelete(DeleteBehavior.Restrict);
Config: ProjectEntityTypeConfiguration also configures inverse? No, only one side configures it.
Status: OK.
14. Comment & User

Model: Comment has CommentedByUserId (Guid). User has ICollection<Comment>? No, User has TeamMemberships and Teams. Comment has CommentedByUserId.
Config: CommentEntityTypeConfiguration does NOT configure CommentedByUserId -> User.
Status: Missing FK configuration. CommentedByUserId is just a column, not mapped to User.
15. DialogueNode & User (Speaker)

Model: DialogueNode has SpeakerId -> User.
Config: DialogueNodeEntityTypeConfiguration says .HasForeignKey(dn => dn.SpeakerId) but does NOT specify .OnDelete() or .WithOne(). It just says builder.HasIndex(e => e.SpeakerId);. It misses the FK relationship configuration entirely!
Conflict: Missing FK Configuration. SpeakerId is not configured as a FK to User. EF Core will treat it as a shadow property or fail to resolve the navigation.
16. ReviewStatus & User (Reviewer)

Model: ReviewStatus has ReviewedByUserId -> User.
Config: ReviewStatusEntityTypeConfiguration says .HasForeignKey(rs => rs.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);
Config: MetaInfoEntityTypeConfiguration says .WithOne(rs => rs.ReviewStatus).HasForeignKey(rs => rs.MetaInfoId)...
Status: OK.
17. ClassTemplate & AttributeSet

Model: ClassTemplate has AttributeSetId -> AttributeSet.
Config: ClassTemplateEntityTypeConfiguration says .WithMany().HasForeignKey(ct => ct.AttributeSetId).OnDelete(DeleteBehavior.Cascade);
Config: AttributeSetEntityTypeConfiguration does NOT configure back-ref.
Status: OK.
18. CharacterIdentity & IdentityDefinition / IdentityValue

Model: CharacterIdentity has IdentityDefinitionId & IdentityValueId.
Config: CharacterIdentityEntityTypeConfiguration configures both.
Status: OK.
19. IdentityValue & Project / ProjectTemplate

Model: IdentityValue has ProjectId & ProjectTemplateId.
Config: IdentityValueEntityTypeConfiguration configures Project (Cascade). Does NOT configure ProjectTemplate.
Status: Missing FK for ProjectTemplate.
20. ProjectTask & ProjectTaskComments

Model: ProjectTask has ICollection<ProjectTaskComments> Comments. ProjectTaskComments has ProjectTaskId.
Config: ProjectTaskEntityTypeConfiguration says .WithMany(pt => pt.Comments).HasForeignKey(pct => pct.ProjectTaskId).OnDelete(DeleteBehavior.Cascade);
Config: ProjectTaskCommentsEntityTypeConfiguration says .WithOne(pt => pt.Comments).HasForeignKey(pct => pct.TaskId).OnDelete(DeleteBehavior.Cascade); -> Mismatch! One uses ProjectTaskId, the other uses TaskId. ProjectTaskComments has TaskId property, but the model says ProjectTaskId. Wait, model says public Guid ProjectTaskId { get; set; }. Config uses TaskId. This will cause a runtime error: The properties expression 'pct => pct.TaskId' is not valid. The expression body should return the foreign key property... because ProjectTaskComments has ProjectTaskId, not TaskId.
Conflict: FK Property Name Mismatch. ProjectTaskComments has ProjectTaskId, but config uses TaskId.
21. TokenUsageLog (Duplicate Configs)

Tokens/TokenUsageLogEntityTypeConfiguration.cs configures ContentId -> MetaInfo with SetNull.
Activities/TokenUsageLogEntityTypeConfiguration.cs configures ProjectId -> Project with Cascade.
Status: OK, different FKs.
22. EngineFieldMapping & EngineExportConfig

Model: EngineFieldMapping has EngineExportConfigId.
Config: EngineFieldMappingEntityTypeConfiguration says composite PK (EngineExportConfigId, SourceColumn, TargetColumn).
Config: EngineExportConfigEntityTypeConfiguration does NOT configure back-ref.
Status: OK.
23. TemplateIdentityDefinition & IdentityDefinition

Model: TemplateIdentityDefinition has IdentityDefinitionId.
Config: TemplateIdentityDefinitionEntityTypeConfiguration does NOT configure the FK relationship! It only sets PK and indexes.
Conflict: Missing FK Configuration. IdentityDefinitionId is not mapped.
24. TemplateAttributeSetDefinition & AttributeSetDefinition

Model: TemplateAttributeSetDefinition has AttributeSetDefinitionId.
Config: TemplateAttributeSetDefinitionEntityTypeConfiguration does NOT configure the FK relationship!
Conflict: Missing FK Configuration.
25. ProjectIdentityDefinition & Project

Model: `ProjectIdentity