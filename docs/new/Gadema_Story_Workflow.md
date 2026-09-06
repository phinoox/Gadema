
***

# Table of Contents

- [GaDeMa Workflow: From Story to System](#gadem-workflow-from-story-to-system)
    - [Core Philosophy](#core-philosophy)
- [1. The Interface Layout: "Focus, Context, Anchor"](#1-the-interface-layout-focus-context-anchor)
- [2. Phase 1: The Skeleton (Project & Story Setup)](#2-phase-1-the-skeleton-project--story-setup)
    - [A. Project Creation: "The Core Compass"](#a-project-creation-the-core-compass)
    - [B. Story Outline & Beats](#b-story-outline--beats)
    - [C. The Tagging Mechanism: Dynamic Contextual Filtering](#c-the-tagging-mechanism-dynamic-contextual-filtering)
- [3. Phase 2: The Writer's Flow (Scene Writing)](#3-phase-2-the-writers-flow-scene-writing)
    - [A. Creating a Scene](#a-creating-a-scene)
    - [B. Writing & Linking](#b-writing--linking)
    - [C. Enriching with Characters/Locations](#c-enriching-with-characterslocations)
- [4. Phase 3: The Bridge (Story to Game Integration)](#4-phase-3-the-bridge-story-to-game-integration)
    - [A. Manual Transfer (Granular Control)](#a-manual-transfer-granular-control)
    - [B. Auto-Parse (Fast Conversion)](#b-auto-parse-fast-conversion)
- [5. Phase 4: Verification & Refinement (Hyperfocus Mode)](#5-phase-4-verification--refinement-hyperfocus-mode)
- [6. Phase 5: The "Loop" (ADHD-Friendly Maintenance)](#6-phase-5-the-loop-adaptive-maintenance)
    - [A. The Save Ritual (Top Panel)](#a-the-save-ritual-top-panel)
    - [B. The Anchor Effect (Right Panel Overviews)](#b-the-anchor-effect-right-panel-overviews)
- [Summary of the User Journey](#summary-of-the-user-journey)

---

# GaDeMa Workflow: From Story to System

> **"Structure should serve the creator, not constrain them."**

This document outlines the user journey from project creation to narrative development and game integration. It is designed for **non-linear thinking**, using **progressive disclosure** and **contextual anchors** to prevent overwhelm.

## 1. The Interface Layout: "Focus, Context, Anchor"

To maintain a clean workspace while keeping the big picture accessible, GaDeMa uses three distinct zones:

| Zone | Purpose | Behavior |
| :--- | :--- | :--- |
| **Top Panel** | **The Compass** | Minimalist status. Current Title/Tags, linked Story Beats, and the **Save Ritual**. Collapsible. |
| **Left Panel** | **The Canvas** | The primary workspace. Switches between **Normal Mode** (Full focus) and **Hyperfocus Mode** (Split view for logic). |
| **Right Panel** | **The Anchor** | A toggleable "Context Drawer." Holds **Overviews** (Chapters, Characters, Beats) so you never lose the big picture. Uses **progressive disclosure** (one view at a time). |

---

## 2. Phase 1: The Skeleton (Project & Story Setup)
*Goal: Establish narrative intent without rigid constraints.*

### A. Project Creation: "The Core Compass"
When starting a new project, the system asks for a **Core Concept** to tailor the workspace:
*   **Primary Format:** Book / Manga / Game / Hybrid (Determines default UI emphasis).
*   **Genre & Theme:** Auto-suggests relevant Story Beats and structural hints.
*   **Tone/Audience:** Influences suggestion engines for pacing and emotional beats.
*   *Note: This is a compass, not a lock. You can switch modes or add elements later.*

### B. Story Outline & Beats
The Outline acts as the **"Map"** of your story, while Beats are the **"Landmarks."**

1.  **StoryOutline (The Map):** Created in the Right Panel's "Overview" tab. This is a flexible document where you write high-level ideas, summaries, and scene prototypes. It serves as the structural backbone of your project.
2.  **StoryBeats (The Landmarks):** Defined as key moments or major plot points (e.g., "The Betrayal"). In the Right Panel, these appear as draggable cards that you can move around to reorganize the flow of your story.
3.  **Multi-View Navigation:** The Outline can be viewed in three ways, instantly switchable:
    *   **Collapsible Tree:** Traditional nested structure.
    *   **Visual Map:** Node-based mind map for non-linear rearranging.
    *   **Timeline/Pacing:** Horizontal scroll showing chronological flow and emotional peaks.

### C. The Tagging Mechanism: Dynamic Contextual Filtering
Tags are not just metadata; they are **interactive filters** that drive the "Multi-View" experience in your Right Panel and Top Panel.

#### 1. In the Writer (Markdown Syntax)
*   **Auto-Tagging:** Use `#tag` syntax in your raw text. The system automatically adds it to the scene's metadata.
*   **Semantic Links:** Use `[[Character Name]]` or `[[Location Name]]`. These act as interactive links that trigger tooltips and overview features.
*   **Visual Feedback:** In the Left Panel, tags appear as small, clickable pills at the top of the editor. Clicking one instantly filters the Right Panel to show related items.

#### 2. In the Right Panel (The Anchor)
*   **Tag Filter Bar:** Located at the top of every Overview tab (Scenes, Beats, Characters).
    *   *Example:* If you are in the "Beats" overview, you can type `#emotional` to see only beats related to emotional arcs.
*   **Dynamic Grouping:** In the "Visual Map" view of your Outline, scenes/beats are automatically clustered by shared tags (e.g., all `#plot-twist` nodes group together visually).

#### 3. In the Top Panel (The Compass)
*   **Quick-Slice Navigation:** A small dropdown in the Top Panel allows you to "Slice" your current view by tag.
    *   *Example:* Clicking `#needs-revision` in the Top Panel instantly switches the Right Panel to a list of all scenes/beats requiring work, regardless of their sequence or chapter.

**Why this benefits ADHD-Friendly Design:**
1.  **Non-Linear Retrieval:** You don't have to remember *where* a scene is (which Chapter/Sequence). You just tag it `#climax` and find it instantly via the filter.
2.  **Progressive Disclosure:** Tags are invisible until you need them. They don't clutter the raw text unless you choose to view them as pills.
3.  **Cross-Domain Linking:** A `#plot-twist` tag in a Book scene can be linked to a `#plot-twist` beat in a Game sequence, creating a unified narrative map across your entire project.

---

## 3. Phase 2: The Writer's Flow (Scene Writing)
*Goal: Enter "Flow State" with zero friction.*

### A. Creating a Scene
1.  Click **"New Scene"** in the Right Panel or via Command Palette (`Ctrl+K`).
2.  A metadata wrapper is created automatically (Title, Slug, Tags).
3.  The Left Panel opens in **Normal Mode** (Full-width Markdown Editor).

### B. Writing & Linking
*   **Markdown Syntax:** Use `[[Character Name]]` to auto-link characters or `#tags` to update metadata.
*   **Linking Beats:** Drag a StoryBeat from the Right Panel onto the canvas.
    *   **Visual Result:** A small, collapsible tag appears in the text: `🎯 Story Beat: The Betrayal`.
    *   **Behavior:** It acts as a structural bookmark. Click to expand details without leaving the writing flow. It does *not* paste raw text or shortcodes.

### C. Enriching with Characters/Locations
1.  Highlight a name (e.g., "John") in the Markdown editor.
2.  A tooltip appears: *"Unknown Character."*
3.  Click **"Create New"** or **"Edit"**.
4.  **Right Panel Action:** The Right Panel slides open to show the **Character Creator/Editor**. You can fill in mechanics and narrative background without leaving the scene.

---

## 4. Phase 3: The Bridge (Story to Game Integration)
*Goal: Convert narrative into interactive logic without rewriting.*

### A. Manual Transfer (Granular Control)
1.  **Highlight** the specific paragraph you want to make interactive.
2.  Click **"Transfer to Game Dialog"**.
3.  **Result:**
    *   A game element is created and linked to that text.
    *   A shortcode marker appears in the text (e.g., `[dialog:ID]`).
    *   **Hyperfocus Mode Activated:** The Left Panel splits (Left: Raw Text, Right: Dialogue Editor).

### B. Auto-Parse (Fast Conversion)
1.  Click **"Analyze & Convert Scene"**.
2.  **Result:**
    *   The system scans for dialogue/actions and highlights regions (Blue = Dialogue, Green = Action).
    *   **Right Panel Action:** You see a list of detected "Game Elements." Drag/drop to reassign them (e.g., "Make this a Cutscene").

---

## 5. Phase 4: Verification & Refinement (Hyperfocus Mode)
*Goal: Ensure Game Logic matches Story Intent.*

In **Hyperfocus Mode**, you are in a **Split View**:
*   **Left Panel:** Original text with markers visible. Editable raw text.
*   **Right Panel (Dialogue Editor):** Shows the metadata for this node plus logic:
    *   **Speaker:** Dropdown to select a character.
    *   **Choices:** Add options ("Attack", "Talk").
    *   **Triggers:** Add actions (`GiveItem`, `SetFlag`).

**Top Panel Updates:**
*   Shows the linked StoryBeat with a status icon.
*   **"Where to Go Next?"** suggestion: *"You've set up the dialogue. Do you want to create the 'Combat' sequence next?"*

---

## 6. Phase 5: The "Loop" (ADHD-Friendly Maintenance)

### A. The Save Ritual (Top Panel)
To prevent "floating away," use the **Ritual Button** in the Top Panel:
1.  **Minor Save:** Happens silently every 30–60 seconds. No UI change.
2.  **Snapshot Save (Intentional):** Click **[📸 Save Snapshot]**.
    *   A small comment field appears: *"Why did you save here?"*
    *   Type one line: *"Finished the betrayal setup."*
    *   **Result:** Creates a versioned milestone. The Right Panel updates to show a **"History"** tab with a horizontal timeline of milestones.

### B. The Anchor Effect (Right Panel Overviews)
To keep you grounded in the big picture:
*   **Chapter Overview:** Toggleable "Tree View" showing which scenes have `Game Logic` vs. raw text.
*   **Character Relations:** A small graph showing how your current character connects to others, updating dynamically as the story progresses.
*   **Beat Map:** A list of StoryBeats with checkboxes. As you link them, they get checked off for a sense of progress.

---

## Summary of the User Journey

1.  **Start:** Create `StoryOutline` (The Map) & `Beats` (The Landmarks) in Right Panel. (The "Why")
2.  **Write:** Create `Scene`, write raw text, link `Beats`. (The "What")
3.  **Enrich:** Highlight names -> Create `Characters/Locations` in Right Panel. (The "Who/Where")
4.  **Convert:** Highlight text -> "Transfer to Game" -> **Hyperfocus Mode** opens. (The "How")
5.  **Refine:** Edit Logic in Right Panel while watching Raw Text on Left. (The Verification)
6.  **Anchor:** Click **[📸 Save Snapshot]** in Top Panel to commit progress and see a timeline of your milestones. (The Ritual)

This workflow keeps the "Story" pure until you are ready to touch it, uses the Right Panel as a "pop-up" workspace so you never lose your place in the narrative, and uses the Top Panel for the **Ritual of Saving** to keep you grounded.