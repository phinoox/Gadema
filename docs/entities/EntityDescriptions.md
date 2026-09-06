# 📊 **GaDeMa Entity Overview** – Brief Summary

---

## **1️⃣ Authentication & Team (3 entities)**

| Entity | Purpose |
| :--- | :--- |
| **User** | Stores user accounts with password hashes, Google OAuth IDs, recovery codes |
| **Team** | Multi-user team projects with name/slug/description |
| **TeamMember** | Tracks who belongs to a team with role (Admin/Editor/Viewer) and join date |

---

## **2️⃣ Projects (1 entity)**

| Entity | Purpose |
| :--- | :--- |
| **Project** | Game development project container with owner, visibility, status, templates, series tracking |

---

## **3️⃣ Content & Media (9+ entities)**

| Entity | Purpose |
| :--- | :--- |
| **MetaInfo** | Main content unit (character/world/mechanic) with title/slug/shortDesc/description/viewMode |
| **StoryOutline** | Narrative structure for outlining before full writing |
| **DialogueBranch** | Visual novel dialogue tree nodes and connections |
| **DialogueNode** | Individual dialogue lines within branches |
| **ExternalReference** | Links to Notion docs, Pinterest boards, YouTube videos for inspiration |
| **MediaAttachment** | File uploads (images/PDFs/audio) attached to content items |
| **Tag** | Reusable tag definitions (MainCharacter, Hero, etc.) |
| **ContentTags** | Junction table linking tags to specific content items |
| **MediaTags** | Tagging for media files with category labels |

---

## **4️⃣ Narrative Structure (3 entities)**

| Entity | Purpose |
| :--- | :--- |
| **StorySequence** | Chapter or scene sequences in the story |
| **StoryBeat** | Plot beats within sequences marking key turning points |
| **LoreEntry** | World-building entries for history, culture, geography |

---

## **5️⃣ Characters (2 entities)**

| Entity | Purpose |
| :--- | :--- |
| **CharacterDetails** | Character attributes (name/level/class/race) stored as child entity with FK as PK |
| **CharacterBackground** | Character backstory, biography, relationships |

---

## **6️⃣ Attributes & Scaling (5 entities)**

| Entity | Purpose |
| :--- | :--- |
| **AttributeSet** | Groups of attributes (Health, Attack, etc.) for character scaling |
| **AttributeDefinition** | Individual attribute definitions within sets |
| **ClassTemplate** | Pre-defined class templates with attribute mappings |
| **ClassTemplateAttribute** | Links class templates to specific attribute values |
| **CharacterAttributes** | Current character attributes (level 10, health 50, etc.) |

---

## **7️⃣ Abilities & GAS (3 entities)**

| Entity | Purpose |
| :--- | :--- |
| **AbilitySet** | Ability categories (Combat/Non-Combat/Hybrid) for RPG systems |
| **AbilityDefinition** | Individual abilities with stats, cooldowns, effects |
| **StatusEffectDefinition** | Buff/debuff status effects that can be applied to characters |

---

## **8️⃣ Tasks & Workflow (6 entities)** ⭐ Renamed from Task → ProjectTask

| Entity | Purpose |
| :--- | :--- |
| **ProjectTask** | Tasks for writing/design work with difficulty, quick win, status filtering |
| **TaskComments** | Comments on specific tasks with team visibility |
| **Comment** | General content comments (not tied to specific task) |
| **ContentVersionLog** | Version history of content changes |
| **ReviewStatus** | Approval workflow for content items |

---

## **9️⃣ Activities & Tokens (2 entities)**

| Entity | Purpose |
| :--- | :--- |
| **ActivityLog** | Project activity feed showing who did what and when |
| **TokenUsageLog** | API token usage tracking for audit/monitoring |

---

## **🔟 Versioning (1 entity)**

| Entity | Purpose |
| :--- | :--- |
| **ContentSnapshot** | Auto/manual snapshots for rollback capability with version tracking |

---

## **1️⃣ Inventory & Endings (2 entities)**

| Entity | Purpose |
| :--- | :--- |
| **InventoryItem** | Collectibles, items, loot systems for game inventory |
| **EndingDefinition** | Game ending definitions for branching storylines |

---

## **2️⃣ Templates (5 entities)**

| Entity | Purpose |
| :--- | :--- |
| **ProjectTemplate** | Pre-defined project structures (RPG/Visual Novel templates) |
| **TemplateAttributeSetDefinition** | Attribute sets defined in template configurations |
| **TemplateClassTemplateDefinition** | Class templates defined in template configurations |
| **TemplateIdentityDefinition** | Identity system definitions for templates |
| **TemplateNarrativeStructure** | Story outline structures included in templates |

---

## **3️⃣ Identity System (3 entities)**

| Entity | Purpose |
| :--- | :--- |
| **ProjectIdentityDefinition** | Defines available identity types per project (Race, Faction, Alignment) |
| **IdentityValue** | Specific values for each identity type (Human, Orc, etc.) |
| **CharacterIdentity** | Links characters to their assigned identities (race/faction/alignment) |

---

## **4️⃣ Engine Integration (3 entities)**

| Entity | Purpose |
| :--- | :--- |
| **EngineExportConfig** | Export configuration for Unity/Unreal integration |
| **EngineFieldMapping** | Maps GaDeMa fields to engine-compatible names |
| **AssetLink** | Cross-platform asset references between projects |

---

## **📋 Summary Table: Entity Categories**

| Domain Category | Count | Key Use Cases |
| :--- | :--- | :--- |
| **User Management** | 3 | Authentication, Team Roles, Project Ownership |
| **Core Content** | 9+ | Writing, Outlines, Dialogue, Media, Tags |
| **Narrative Structure** | 3 | Sequences, Beats, Lore for Story Design |
| **Character Systems** | 5 | RPG attributes, Scaling, Classes, Details |
| **Abilities System** | 3 | Combat/Non-Combat abilities with GAS support |
| **Task Management** | 6 | ADHD-friendly task filtering, Review workflows |
| **Version Control** | 2 | Auto-save snapshots, Rollback capability |
| **Templates & Identity** | 10 | Pre-structures and Character Identity tracking |
| **Engine Export** | 3 | Unity/Unreal compatibility mappings |
| **Monitoring** | 2 | Activity feeds, Token usage logging |

---

## **🎯 Key Design Patterns:**

✅ **FK as PK**: CharacterDetails, CharacterBackground use FK as primary key (child entities)  
✅ **Domain Clustering**: All ~57 entities organized in domain-separated folders  
✅ **Configuration Files**: Each entity has dedicated `XxxEntityTypeConfiguration.cs` file  
✅ **Cascade Deletes**: MetaInfo cascades to MediaAttachments; Tasks restrict for history  
✅ **Hybrid Response Patterns**: API endpoints return RAW (data) or WRAPPED (confirmation/error)  

This architecture supports the MVP goal of creating a comprehensive, maintainable game development management tool! 🎮✨