# ✍️ Content Items & Narrative Structure

This guide explains how Anima Lab manages diverse types of information using a unified polymorphic model, and how this content is organized into narrative hierarchies.

## 1. The Polymorphic Content Model (`MetaInfo`)

Instead of creating separate tables for every type of thing you might want to document (Characters, Locations, Items, etc.), Anima Lab uses a single, central table called `MetaInfo`. This approach avoids "table explosion" and allows all content items to share common infrastructure like tagging, media attachments, and versioning.

### 1.1 The Core Entity: `MetaInfo`
Every piece of content—whether it's a character profile, a world location, or a game mechanic—is a `MetaInfo` entry.

| Field | Purpose |
| :--- | :--- |
| **`ContentType`** | The "Discriminator" that defines what the item is (e.g., Character, World, Mechanic). |
| **`Title` & `Slug`** | The primary identification and URL-friendly identifier for the item. |
| **`Status`** | Tracks the lifecycle: `Draft` $\rightarrow$ `InProgress` $\rightarrow$ `UnderReview` $\rightarrow$ `Published`. |
| **`ViewMode`** | Determines how the content is rendered: `PrivateWriting` (Editor) vs `Presentation` (Reader). |
| **`Version`** | An integer counter used to track revisions and support rollbacks. |

### 1.2 Type-Specific Data (Polymorphism in Action)
While all items share a common base, specific details are stored in "Child Tables" linked via `MetaInfoId`. This prevents the main table from becoming too wide/bloated while allowing rich, specialized data.

**Example: Character vs. World**
* **A Character item**: The `MetaInfo` entry holds the name and title. Its specific properties (like *Race*, *Class*, or *Level*) are stored in the linked `CharacterDetails` table.
* **A World item**: The `MetaInfo` entry holds the location name. Its specific properties (like *Climate* or *Geography*) are stored in associated `LoreEntry` tables.

---

## 2. Narrative Hierarchy

Anima Lab supports multiple ways to structure content, moving from broad containers down to atomic units of work.

### 2.1 The Hierarchy Chain
The system follows a natural progression for storytellers:

**Project $\rightarrow$ StorySequence $\rightarrow$ StoryBeat**

1.  **Project**: The top-level container (e.g., "The Dragon Chronicles").
2.  **StorySequence**: An organizational bucket representing acts, chapters, or major movements (e.print `Act 1`, `Chapter 3`). Sequences can optionally have a parent sequence to create a nested tree.
3.  **StoryBeat**: The atomic unit of work—a specific scene, moment, or beat within a sequence.

### 2.2 Branching Narratives (Dialogue Trees)
For interactive content like dialogue, `MetaInfo` acts as the root for a branching structure:
* A `MetaInfo` entry of type `PlotPoint` can serve as the starting node for a tree of `DialogueNodes`.
* This allows writers to create complex, choice-driven narratives that are still managed under the umbrella of a single content item.

---

## 3. Shared Infrastructure (Cross-Cutting Concerns)

Regardless of their type, all content items benefit from these universal features:

### 3.1 Media & External References
* **Media Attachations**: Every `MetaInfo` can have multiple files (images, PDFs, voice clips) attached to it.
* **External References**: Link your content to the wider web, such as Pinterest boards, Google Docs, or research papers.

### 3.2 Tagging System
All items participate in a many-to-many relationship with `Tags`. This allows you to categorize disparate content (e.g., tagging both a "Character" and a "Location" with the tag `#HighFantasy").

### 3.3 Versioning & Audit Trail
The system maintains an automatic history of changes:
* **Snapshots**: Every major change creates a `ContentSnapshot` (a JSON blob of the entity's state).
* **Rollbacks**: You can instantly revert a content item to any previous version using these snapshots.

---

## 4. Summary Table: Content Types

| Type | Typical Child Entities | Primary Use Case |
| :--- | :--- | :--- |
| `Character` | `CharacterDetails`, `CharacterAttributes` | RPG-style character profiles and stats |
| `World` | `StorySequence`, `LoreEntry` | Setting descriptions, locations, and geography |
| `Mechanic` | `ClassTemplate`, `AbilityDefinition` | Game rules, systems, and abilities |
| `PlotPoint` | `DialogueBranch/Node` | Narrative beats, quests, and branching dialogue |

***
*Last Updated: [Date]*
