You are absolutely right. The original document was detailed because it guided the user through the *experience* of the tool. I condensed it too much in the previous version.

Here is the **full, expanded v2 documentation**, matching the original's depth, tone, and structure, while fully integrating your new **Story/Outline/Scene** hierarchy.

***

# GaDeMa Workflow: From Story to System (v2)

> **"Structure should serve the creator, not constrain them."**

This document outlines the user journey from project creation to narrative development and game integration. It is designed for **non-linear thinking**, using **progressive disclosure** and **contextual anchors** to prevent overwhelm.

## 1. The Interface Layout: "Focus, Context, Anchor"

To maintain a clean workspace while keeping the big picture accessible, GaDeMa uses three distinct zones:

| Zone | Purpose | Behavior |
| :--- | :--- | :--- |
| **Top Panel** | **The Compass** | Minimalist status. Current **Story**, **Story Health**, linked **Story Beats**, and the **Save Ritual**. Collapsible. |
| **Left Panel** | **The Canvas** | The primary workspace. Switches between **Normal Mode** (Full focus) and **Hyperfocus Mode** (Split view for logic). |
| **Right Panel** | **The Anchor** | A toggleable "Context Drawer." Holds **Overviews** (StoryMap, Beats, Scenes) so you never lose the big picture. Uses **progressive disclosure** (one view at a time). |

---

## 2. Phase 1: The Skeleton (Story & Outline Setup)
*Goal: Establish narrative intent without rigid constraints.*

### A. The Core Compass: "Story" Creation
**The Shift:** A Project is no longer just a flat list. It is a container for one or more **Stories**.

When starting a new project, you define the **Story** first. This is your "Container."
*   **The Glue:** The **Story** entity acts as the glue for your narrative. It holds your Map (Outline) and your Landmarks (Beats) together.
*   **Multiple Timelines:** If you are writing a Game with multiple timelines or a Book with multiple POVs, you simply create a **New Story** for each one.
*   **Context Isolation:** Use the **Story Switcher** in the Top Panel to instantly change your workspace. This ensures you never mix up plot points between different timelines.

### B. The Map: "StoryOutline" & "Smart Split"
The Outline is your flexible roadmap. It starts as a single block but grows with you.

1.  **The "One Big Blob":** When you start, you get one `StoryOutlineSection`. Write a massive summary without worrying about structure.
2.  **The "Smart Split" Feature:** When you are ready to organize:
    *   **Highlight** a paragraph in your outline.
    *   Click the **"+"** button.
    *   **Visual Result:** The text you highlighted instantly detaches and becomes a new `OutlineSection` card below. A new blank card appears for you to continue.
    *   **Reorder:** Drag and drop sections to reorganize your Acts or Chapters instantly.

### C. The Landmarks: "StoryBeats" (Global Tracking)
`StoryBeats` are the major events in your narrative (e.g., "The Betrayal," "The First Kiss").

*   **Global to Story:** Beats live inside the **Story** container, not the Outline. This allows you to plan the "What" (Beats) separately from the "How" (Outline).
*   **Story Health (The Dopamine Loop):**
    *   **The Visual:** A progress bar in the Top Panel shows how many of your Beats have been covered by Scenes.
    *   **The Benefit:** Instant dopamine. It turns the abstract Outline into a tangible checklist that updates automatically as you write.

---

## 3. Phase 2: The Writer's Flow (Scene Writing)
*Goal: Enter "Flow State" with zero friction.*

### A. The Nexus: "Scene"
The **Scene** is the central hub where the Story meets the Game.

1.  **Create a Scene:** Click **"New Scene"** in the Right Panel or via Command Palette (`Ctrl+K`).
2.  **Link a Beat:** In the Top Panel, select a **StoryBeat** (e.g., "The Betrayal"). This sets the "Current Beat" context.
3.  **Write:** The Left Panel opens in **Normal Mode** (Full-width Markdown Editor).

### B. Writing & Linking
*   **Markdown Syntax:** Use `#tags` to update metadata and `[[Character Name]]` to auto-link characters.
*   **Contextual Anchors:** The Top Panel always shows you the **Current Beat**, reminding you *why* you are writing this specific scene.
*   **Visual Feedback:** If you are "behind" on a Beat, the icon in the Top Panel turns yellow. Click it to jump to the next unfinished landmark.

---

## 4. Phase 3: The Bridge (Story to Game Integration)
*Goal: Convert narrative into interactive logic without rewriting.*

### A. Manual Transfer (Granular Control)
1.  **Highlight** the specific paragraph you want to make interactive.
2.  Click **"Transfer to Game Dialog"**.
3.  **Result:** A **SceneSegment** is created.
    *   **The Bridge:** A `SceneSegment` is the "translator" between your Story and the Game Engine.
    *   **GameKeyEvent:** Inside the Segment, you define specific mechanics (e.g., "GiveItem", "TriggerCombat").

### B. Hyperfocus Mode Activated
The Left Panel splits:
*   **Left:** Original text with markers visible.
*   **Right:** The **Segment Editor** showing your `GameKeyEvent`s and logic.

---

## 5. Phase 4: Verification & Refinement
*Goal: Ensure Game Logic matches Story Intent.*

**Top Panel Updates:**
*   Shows the linked **StoryBeat** with a status icon.
*   **"Where to Go Next?"** suggestion: *"You've covered the 'Betrayal' beat. Do you want to create the 'Aftermath' scene next?"*

---

## 6. Phase 5: The "Loop" (ADHD-Friendly Maintenance)

### A. The Save Ritual (Top Panel)
To prevent "floating away," use the **Ritual Button** in the Top Panel:
1.  **Minor Save:** Happens silently every 30–60 seconds.
2.  **Snapshot Save (Intentional):** Click **[📸 Save Snapshot]**.
    *   **Result:** Creates a versioned milestone. The Right Panel updates to show a **"History"** tab.

### B. The Anchor Effect (Right Panel Overviews)
*   **Story Health:** Check your global progress against the **StoryBeats**.
*   **Scene Map:** See which Scenes are linked to which Beats.

---

## Summary of the User Journey

1.  **Start:** Create a **Story** (The Container).
2.  **Map:** Draft your **Outline** using the **Smart Split** to organize ideas.
3.  **Landmarks:** Define **StoryBeats** and track them with **Story Health**.
4.  **Write:** Create a **Scene**, link it to a Beat, and write the **RawText**.
5.  **Bridge:** Use **SceneSegments** to add **GameKeyEvents** (Logic).
6.  **Anchor:** Click **[📸 Save Snapshot]** to commit progress.