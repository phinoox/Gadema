// ─── AUTHENTICATION ───────────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(Team), typeof(User))]
public partial class TeamMember { ... }

[DependencyResolver.ModelDependency(typeof(User))]
public partial class Team { ... }

[DependencyResolver.ModelDependency(typeof(User))]
public partial class UserProviderLink { ... }

// ─── PROJECTS ─────────────────────────────────────────────────────────────

// ProjectSeries: NO dependencies (root)

[DependencyResolver.ModelDependency(typeof(Project), typeof(Team))]
public partial class ProjectTeam { ... }

[DependencyResolver.ModelDependency(typeof(Project), typeof(ProjectTag))]
public partial class ProjectTagRelation { ... }

[DependencyResolver.ModelDependency(typeof(User))]
public partial class Project { ... }

// ─── NARRATIVE ─────────────────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(StorySequence))]
public partial class StoryBeat { ... }

[DependencyResolver.ModelDependency(typeof(StorySequence), typeof(MetaInfo))]
public partial class StoryOutline { ... }

[DependencyResolver.ModelDependency(typeof(Project))]
public partial class StorySequence { ... }

[DependencyResolver.ModelDependency(typeof(Project), typeof(MetaInfo))]
public partial class LoreEntry { ... }

// ─── CONTENT / ENGINE INTEGRATION ──────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(Project))]
public partial class MetaInfo { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class AssetLink { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class MediaAttachment { ... }

// ActivityLog: no FK (uses ProjectId as string/identifier, not EF FK)
// TokenUsageLog has a nullable FK to ProjectToken — include it anyway for safety
[DependencyResolver.ModelDependency(typeof(ProjectToken))]
public partial class TokenUsageLog { ... }

// ─── TASKS ─────────────────────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(Project), typeof(MetaInfo))]
public partial class ProjectTask { ... }

[DependencyResolver.ModelDependency(typeof(ProjectTask))]
public partial class ProjectTaskComments { ... }

// ─── ATTRIBUTES / ABILITIES ────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(Project), typeof(MetaInfo))]
public partial class AttributeSet { ... }

[DependencyResolver.ModelDependency(typeof(Project), typeof(MetaInfo))]
public partial class AbilitySet { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class AbilityDefinition { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class StatusEffectDefinition { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class AttributeDefinition { ... }

// ─── CHARACTERS / IDENTITY ────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(MetaInfo), typeof(AttributeDefinition))]
public partial class CharacterAttributes { ... }

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class CharacterIdentity { ... }

// ─── VERSIONING / STORY ───────────────────────────────────────────────────

[DependencyResolver.ModelDependency(typeof(MetaInfo))]
public partial class ContentVersionLog { ... }

[DependencyResolver.ModelDependency(typeof(Project))]
public partial class ProjectToken { ... }

// DialogueNode, DialogueBranch: NO dependencies (only scalar properties)

Authentication:       User          (root)
                     TeamMember     → [Team, User]
                     Team           → [User]
                     UserProviderLink → [User]

Projects:             ProjectSeries (root)
                     ProjectTag      (root)
                     Project         → [User]
                     ProjectTeam     → [Project, Team]
                     ProjectTagRelation → [Project, ProjectTag]

Narrative:            StorySequence  → [Project]
                     StoryOutline    → [StorySequence, MetaInfo]
                     StoryBeat       → [StorySequence]
                     LoreEntry       → [Project, MetaInfo]

Content/Engine:       MetaInfo      → [Project]
                     AssetLink         → [MetaInfo]
                     MediaAttachment   → [MetaInfo]
                     ActivityLog       → [Project]
                     TokenUsageLog     → [ProjectToken]

Tasks:                ProjectTask        → [Project, MetaInfo]
                     ProjectTaskComments → [ProjectTask]

Attributes/Abilities: AttributeSet    → [Project, MetaInfo]
                     AbilitySet         → [Project, MetaInfo]
                     AbilityDefinition  → [MetaInfo]
                     StatusEffectDef    → [MetaInfo]
                     AttributeDef       → [MetaInfo]

Characters:           CharacterAttributes   → [MetaInfo, AttributeDefinition]
                     CharacterIdentity       → [MetaInfo] (nullable FKs)

Versioning/Story:     ContentVersionLog      → [MetaInfo]
                     ProjectToken             → [Project]

# Gadema.Core - Foreign Keys & Navigation Properties Map

## Legend
- **FK** = ForeignKey (property holding the FK value)
- **NP** = Navigation Property (virtual property referencing related entity)
- **Cascade** = Cascade delete behavior inferred from configuration patterns

---

## 1. User
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→User | `TeamMember.UserId` | 1:1 | `TeamMember.User` | Cascade (restrict) |
| FK→User | `Team.CreatedByUserId` | 1:1 | `Team.CreatedByUser` | Restrict |
| PK→Many | `Id` (PK) | Many-to-Many via TeamMemberships | — | — |

---

## 2. Team
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Team | `TeamMember.TeamId` | 1:1 | `TeamMember.Team` | Cascade |
| FK→User | `CreatedByUserId` | 1:1 | `CreatedByUser` | Restrict |
| PK→Many | `Id` (PK) | One-to-Many | `TeamMembers` | Cascade |

---

## 3. TeamMember
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Team | `TeamId` | Many:1 | `Team` | Cascade |
| FK→User | `UserId` | Many:1 | `User` | Restrict |

---

## 4. Project
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→User | `OwnerId` | Many:1 | `Owner` | Cascade |
| FK→ProjectSeries | `ProjectSeriesId` | Many:1 | `ProjectSeries` | Cascade |
| PK→Many | `Id` (PK) | One-to-Many | `MetaInfos` | — |
| PK→Many | `Id` (PK) | One-to-Many | `Sequences` | — |
| PK→Many | `Id` (PK) | One-to-Many | `Tasks` | — |
| PK→Many | `Id` (PK) | One-to-Many | `ProjectTeams` | Cascade |

---

## 5. ProjectSeries
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| PK→Many | `Id` (PK) | One-to-Many | `Projects` | — |

---

## 6. StorySequence
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| FK→ParentSequence | `ParentSequenceId` | Self-Ref (Many-to-One) | `ParentSequence` | Cascade |
| PK→Many | `Id` (PK) | One-to-Many | `ChildSequences` | — |
| PK→Many | `Id` (PK) | One-to-Many | `Outlines` | — |
| PK→Many | `Id` (PK) | One-to-Many | `Beats` | — |

---

## 7. StoryBeat
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→StorySequence | `SequenceId` | Many:1 | `StorySequence` | Cascade |

---

## 8. StoryOutline
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→StorySequence | `SequenceId` | Many:1 | `StorySequence` | Cascade |
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 9. LoreEntry
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 10. ProjectTask
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 11. ProjectTaskComments
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→ProjectTask | `ProjectTaskId` | Many:1 | `ProjectTask` | Cascade |

---

## 12. AttributeSet
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 13. AbilitySet
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 14. AbilityDefinition
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 15. StatusEffectDefinition
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 16. AttributeDefinition
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 (optional) | `MetaInfo` | — |

---

## 17. CharacterAttributes
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| **Composite PK** → | `(MetaInfoId, AttributeDefinitionId)` | Many-to-Many | — | — |
| FK→AttributeDefinition | `AttributeDefinitionId` | One:One (via composite) | `AttributeDefinition` | Cascade |

---

## 18. CharacterIdentity
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 | `MetaInfo` | Cascade |
| FK→IdentityDefinition | `IdentityDefinitionId` | Many:1 (optional) | `IdentityDefinition` | — |
| FK→IdentityValue | `IdentityValueId` | Many:1 (optional) | `IdentityValue` | — |

---

## 19. MetaInfo
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |
| PK→Many | `Id` (PK) | One-to-Many | `ContentVersionLogs` | — |
| PK→Many | `Id` (PK) | One-to-Many | `AssetLinks` | — |
| PK→Many | `Id` (PK) | One-to-Many | `MediaAttachments` | — |
| PK→Many | `Id` (PK) | One-to-Many | `MetaInfoTags` | — |

---

## 20. ProjectToken
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | Many:1 | `Project` | Cascade |

---

## 21. ProjectTagRelation (Junction Table)
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | One:One (junction side) | `Project` | Cascade |
| FK→ProjectTag | `ProjectTagId` | One:One (junction side) | `ProjectTag` | Cascade |

---

## 22. ProjectTag
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| PK→Many | `Id` (PK) | One-to-Many | `ProjectTags` (junction refs) | — |

---

## 23. AssetLink
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 | `MetaInfo` | Cascade |

---

## 24. MediaAttachment
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→MetaInfo | `MetaInfoId` | Many:1 | `MetaInfo` | Cascade |

---

## 25. ActivityLog
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→Project | `ProjectId` | One:One (log side) | `Project` | Restrict |

---

## 26. TokenUsageLog
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→ProjectToken | `ProjectTokenId` | One:One (log side) | `ProjectToken` | Restrict |

---

## 27. UserProviderLink
| Direction | FK / PK | Type | NP Name | Behavior |
|-----------|---------|------|---------|----------|
| FK→User | `UserId` | Many:1 | `User` | Cascade |

---

## Summary by Relationship Pattern

| Pattern | Count |
|---------|-------|
| **Cascade Delete** (FK → NP) | ~35 relationships |
| **Restrict Delete** (FK → NP) | 4 relationships (`CreatedByUserId`, `ReviewStatus`) |
| **Optional FK** (`null!` allowed) | ~12 properties with nullable FKs |
| **Composite PK** junction tables | 2 tables (`CharacterAttributes`, `ProjectTagRelation`) |

---

## Key Design Patterns Observed

1. **Cascade on child collections** — All collection navigation properties (e.g., `Tasks`, `MetaInfos`, `ChildSequences`) use `.List<>()` with cascade configured in EF Core configuration files.
2. **Restrict on "owner" relationships** — `CreatedByUserId` uses restrict to preserve historical audit trails.
3. **Composite PK for junctions** — `CharacterAttributes` and `ProjectTagRelation` use FK-as-PK pattern (though `CharacterAttributes` is explicitly composite).
4. **Optional content linking** — Many models reference `MetaInfo` via nullable FK (`Guid?`) allowing flexible attachment without mandatory parent.