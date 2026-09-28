# 🏷️ Enums & Type Definitions

This guide provides a comprehensive reference for the enumerations used throughout Anima Lab, mapping them to their respective domains and explaining their design rationale.

## 1. Overview

Anima Lab uses strongly-typed enumerations to ensure data integrity, facilitate efficient database storage, and enable bitwise operations for multi-dimensional classification. All enums are defined in `src/Gadema.Core/Enums/`.

### Core Design Principles:
* **Dense Integers**: Enums use consecutive integers starting from `0` for efficient SQL storage and bitwise logic.
* **[Flags] Attribute**: Used for multi-dimensional classification (e.s., character identity) where an entity can belong to multiple categories simultaneously.
* **Actionable States**: Every enum value represents a valid, actionable state; there are no "Unknown" or "None" sentinel values.

---

## 2. Enum Registry by Domain

### 🧩 Content Model Classification
Used to classify the nature of content items and their lifecycle states.

| Enum Name | Usage Context | Examples |
| :--- | :--- | :--- |
| `ContentTypeEnum` | Determines the kind of content (e.g., Character, World). | `Character`, `World`, `Mechanic` |
| `ContentStatusEnum` | Tracks the lifecycle stage of an item. | `Draft`, `Published`, `Archived` |
| `ViewModeEnum` | Controls UI rendering mode. | `PrivateWriting` (Editor), `Presentation` (Reader) |

### 🛠️ Task & Workflow Management
Designed with ADHD-friendly principles to support energy-matching and momentum building.

| Enum Name | Usage Context | Examples |
| :--- | :--- | :--- |
| `TaskStatusEnum` | Tracks the workflow pipeline. | `Backlog`, `InProgress`, `Done` |
| `TaskDifficultyEnum` | Matches task complexity to user energy levels. | `Easy` (Quick Win), `Medium`, `Hard` |
| `TaskPriorityEnum` | Drives scheduling and urgency. | `High`, `Medium`, `Low` |

### 🎭 Character Identity System
Uses the `[Flags]` attribute to support multi-dimensional character typing.

| Enum Name | Usage Context | Examples (Bitwise combinations) |
| :--- | :--- | :--- |
| `IdentityTypeEnum` | Multi-dimensional identity (Race, Faction, Alignment). | `Human (Race) \| Alliance (Faction)` |
| `IdentityDefinitionType` | Categorizes the type of identity being defined. | `Race`, `Faction`, `Alignment` |

### 🏗️ Project & Team Structure
Defines how projects are owned and managed within a collaborative environment.

| Enum Name | Usage Context | Examples |
| :--- | :--- | :--- |
| `OwnerTypeEnum` | Determines if the owner is a User or a Team. | `User`, `Team` |
| `ProjectStatusEnum` | Tracks project lifecycle stages. | `Draft`, `InProgress`, `Published` |
| `ProjectVisibilityEnum` | Controls access levels for project content. | `Private`, `Public`, `Anonymous` |
| `TeamMemberRoleEnum` | Defines permission hierarchy within a team. | `Admin`, `Editor`, `Viewer` |

### 📜 Narrative & World Building
Tracks the development and classification of story elements.

| Enum Name | Usage Context | Examples |
| :--- | :--- | :--- |
| `OutlineStatusEnum` | Tracks narrative planning stages. | `Draft`, `Outlined`, `Structured` |
| `LoreTypeEnum` | Categorizes world-building entries. | `History`, `Mythology`, `Geography` |

### 📦 Export & Versioning
Controls how data is transformed for external use and how snapshots are stored.

| Enum Name | Usage Context | Examples |
| :--- | :--- | :--- |
| `ExportFormatEnum` | Defines target file formats. | `Pdf`, `Json`, `Markdown` |
| `SnapshotTypeEnum` | Classifies versioning snapshots. | `Full`, `Delta`, `Comparison` |

---

## 3. Implementation Reference: Bitwise Identity

The `IdentityTypeEnum` uses the `[Flags]` attribute to allow a single integer to represent multiple simultaneous identities.

```csharp
// src/Gadema.Core/Enums/IdentityTypeEnum.cs
[Flags]
public enum IdentityTypeEnum : int
{
    Race      = 1,   // 0001
    Faction   = 2,   // 0010
    Alignment = 4,   // 0100
    Guild     = 8    // 1000
}

// Example: A character that is both Human (Race) and part of the Alliance (Faction).
// IdentityValue = 3 (0011 in binary)
```

---

## 4. Developer Guide: Adding New Enums

To maintain system integrity, follow these rules when adding new enumerations:

1.  **Location**: Place the new enum in `src/Gadema.Core/Enums/`.
2.  **Integer Values**: Always start at `0` and use consecutive integers.
3.  **Documentation**: Add a brief comment describing the purpose of each value.
4.  **Validation**: If used in a model, ensure the property is decorated with `[EnumDataType(typeof(YourEnum))]`.
5.  **Migrations**: When adding new values to an existing enum, ensure you create a database migration to update the type if using PostgreSQL enums.

***
*Last Updated: [Date]*
