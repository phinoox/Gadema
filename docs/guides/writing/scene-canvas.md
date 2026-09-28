# The Scene (The Writing Canvas)

The `Scene` is the fundamental unit of work within the GaDeMa system. It is the primary interface for the writer, acting as the "canvas" where raw narrative text meets structural markers and dynamic character state.

## 🛠️ Implementation Details

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | Unique identifier for the scene. |
| `RawText` | `string` | The actual Markdown content of the scene. |
| `OrderIndex` | `int?` | The sequence of this scene within its parent chapter. |
| `StoryChapterId`| `Guid` | Foreign Key to the parent `StoryChapter`. |

## 🏗️ The Three Pillars of a Scene

A scene in GaDeMa is more than just text; it serves three critical roles that drive the narrative forward:

### 1. The Canvas (Content)
The `RawText` property holds the primary prose, dialogue, or script. This is where the writer's creative work resides.

### 2. The Structural Markers (Segments & Beats)
To prevent "fragile text indexing" (where a small change in text breaks all your links), scenes use two types of structural markers:
- **SceneSegments**: These are non-textual markers that index interactive elements (like a specific dialogue choice or an action trigger) within the scene. They allow the system to "know" where things happen without parsing raw text every time.
- **StoryBeats**: A scene can fulfill one or more `StoryBeat` milestones. This allows writers to anchor major narrative turning points (e.s., "The Hero's Call to Adventure") directly to the scenes where they occur.

### 3. The Dynamic Driver (State & Relations)
The scene is the engine of character progression. As a story unfolds, the `Scene` records:
- **CharacterState**: Changes in physical or mental status (e.g., "Injured", "Level Up").
- **CharacterRelation**: Shifts in social dynamics (e.s., "Becomes Allies", "Develops Grudge").

## ⚠️ Architectural Notes

### The Nexus Pattern
The `Scene` acts as a nexus between the **Static Narrative** (the story arc and beats) and the **Dynamic Reality** (character states and relations). This design ensures that narrative progression is not just an abstract concept, but a measurable consequence of the scenes being written.
