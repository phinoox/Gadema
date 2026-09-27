# GaDeMa Model Architecture

> **"The data model must serve the user's creative flow, not constrain it."**

This document defines the entity relationships and architectural decisions for the GaDeMa application. It bridges the gap between the **User Workflow** (`Gadema_Story_Workflow.md`) and the actual database schema.

---

## Table of Contents
- [1. Core Philosophy](#1-core-philosophy)
- [2. Project & Setup (Phase 1)](#2-project--setup-phase-1)
- [3. Narrative Structure: The "Map & Landmark" Model (Phase 2)](#3-narrative-structure-the-map--landmark-model-phase-2)
- [4. The Bridge: Markers & Segments (Phase 3 & 4)](#4-the-bridge-markers--segments-phase-3--4)
- [5. Character Evolution (Phase 5)](#5-character-evolution-phase-5)
- [6. The Save Ritual (Phase 5)](#6-the-save-ritual-phase-5)
- [7. Summary of Entity Relationships](#7-summary-of-entity-relationships)

---

## 1. Core Philosophy

The GaDeMa data model is designed around three key principles:
1.  **Robustness over Performance:** We use "Unique Token" markers for shortcodes rather than text indices to ensure links remain intact during editing.
2.  **Non-Linear Narrative:** The model supports Many-to-Many relationships between Scenes and Beats, allowing flexible story structures.
3.  **Progressive Disclosure:** Entities are designed to hold minimal data by default, with rich details (like Character Relations or Snapshots) stored in specialized junction tables.

---

## 2. Project & Setup (Phase 1)

### `Project` Model
The root entity of the application. It defines the "Core Compass" that tailors the workspace.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | Primary Key |
| `Name` | string | The project title |
| `PrimaryFormat` | Enum | Book / Manga / Game / Hybrid (Determines UI emphasis) |
| `Genre` | string | Auto-suggests relevant Story Beats |
| `Theme` | string | Influences structural hints |
| `Tone` | string | Influences suggestion engines for pacing |
| `Audience` | string | Target audience demographics |

---

## 3. Narrative Structure: The "Map & Landmark" Model (Phase 2)

This hierarchy defines how the story is planned and executed.

### Hierarchy Flow
`Project` → `StoryOutline` → `StoryBeat` ↔ `Scene`

### `StoryOutline` (The Map)
Acts as the container for high-level narrative intent. It is a distinct model with its own rich text content.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | Primary Key |
| `ProjectId` | FK | Links to `Project` |
| `RawText` | string | The "Prototype" document (Markdown) |
| `CreatedAt` | DateTime | Creation timestamp |

### `StoryBeat` (The Landmark)
Represents major keypoints or plot twists within the Outline. These become the "Cards" in the Right Panel visualization.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | Primary Key |
| `StoryOutlineId` | FK | Links to `StoryOutline` |
| `Title` | string | The name of the beat (e.g., "The Betrayal") |
| `Description` | string | Detailed description of the beat's intent |
| `OrderIndex` | int | Determines visual order in the Timeline/Map views |

### `SceneStoryBeatMapping` (Junction Table)
Enables the Many-to-Many relationship between Scenes and Beats. A single scene can fulfill multiple narrative beats, and a beat can be referenced by many scenes.

| Field | Type | Description |
| :--- | :--- | :--- |
| `SceneId` | FK | Links to `Scene` |
| `StoryBeatId` | FK | Links to `StoryBeat` |

---

## 4. The Bridge: Markers & Segments (Phase 3 & 4)

### `ContentSegment` Model (The Index)
Handles the "Unique Token" markers (e.g., `[dialog:123]`) used in Markdown text. It acts as a robust index of "interesting parts" within a scene without storing fragile text indices.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | The Unique Token (used in shortcodes) |
| `SceneId` | FK | Links to the parent `Scene` |
| `Type` | Enum | Dialogue / StoryBeat / Custom Segment |
| `CreatedAt` | DateTime | When the segment was created/parsed |

### `MetaInfo` Integration
Shortcodes act as keys to look up data in the `MetaInfo` entity. When a shortcode is parsed, the system uses the ID to find the corresponding `MetaInfo` entry for display or logic.

---

## 5. Character Evolution (Phase 5)

### `CharacterRelation` Model
Tracks how relationships between characters change over time. This is an event-driven junction table.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | Primary Key |
| `SourceCharacterId` | FK | The character initiating the relationship |
| `TargetCharacterId` | FK | The character receiving the relationship |
| `RelationType` | Enum | Ally / Enemy / Family / Romantic |
| `TriggerSceneId` | FK | The scene where this relationship evolved |

---

## 6. The Save Ritual (Phase 5)

### `ContentSnapshot` Model (Refined)
Backs the "Save Ritual" in the Top Panel. Stores intentional milestones with context.

| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | Guid/Int | Primary Key |
| `MetaInfoId` | FK | Links to the saved content item |
| `Comment` | string | User-provided reason for saving ("Why did you save here?") |
| `VersionNumber` | int | Sequential versioning for timeline display |
| `CreatedAt` | DateTime | Timestamp of the snapshot |

---

## 7. Summary of Entity Relationships

| Parent Entity | Child Entity | Relationship Type | Purpose |
| :--- | :--- | :--- | :--- |
| `Project` | `StoryOutline` | One-to-Many | Container for narrative intent |
| `StoryOutline` | `StoryBeat` | One-to-Many | Landmarks within the map |
| `StoryBeat` | `Scene` | Many-to-Many | Scenes fulfilling beats (via Junction) |
| `Scene` | `ContentSegment` | One-to-Many | Indexing markers in raw text |
| `Character` | `CharacterRelation` | One-to-Many | Tracking relationship evolution |
| `MetaInfo` | `ContentSnapshot` | One-to-Many | Backing the Save Ritual |

---

*For the user-facing workflow, see [GaDeMa Story Workflow](./Gadema_Story_Workflow.md).*