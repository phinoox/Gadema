# 🎭 Narrative Design Philosophy

> "Structure should serve the writer, not constrain them. The tool must support non-linear thinking without forcing a single path."

This document outlines the core philosophy behind Anima Lab's approach to story authoring and game design. It explains why we chose a graph-based model over traditional hierarchies and how our architecture supports both the "systems thinker" and the "discovery writer."

---

## 1. The Core Principle: Graph, Not Tree

Traditional content management systems typically use a **Tree Structure**: a rigid, hierarchical parent-child relationship where every child must belong to exactly one parent (e.g., Chapter $\rightarrow$ Scene $\rightarrow$ Beat). This works for linear stories but creates massive friction for non-linear thinkers.

Anima Lab uses a **Graph-Based Model**. In our system, relationships are links, not rigid constraints.

### Why this matters:
* **No "Wrong" Entry Point**: A writer can start with a single beat, a scene, or a high-level sequence. You don't have to build the foundation before you can add the roof.
* **Non-Linear Flexibility**: A scene can be moved between sequences without breaking its internal structure or losing its associated beats.
* **Multi-Dimensionality**: A single element (like a "Red Thread" motif) can be linked across multiple scenes, creating a web of connections rather than a simple list.

| Feature | Rigid Tree Model | Anima Lab Graph Model |
| :--- | :--- | :--- |
| **Structure** | Forced hierarchy (Parent $\rightarrow$ Child) | Flexible links (Many-to-Many) |
| **Editing** | Moving items risks breaking the tree | Moving items is a simple re-link |
| **Entry Point** | Must follow top-down path | Enter anywhere, build outward |

---

## 2. Terminology & Scale Invariance

To support both game designers and creative writers, we use scale-invariant terminology. This allows the same logic to apply whether you are organizing a game campaign or a novel.

### The Hierarchy of Intent
Regardless of the scale, content is organized into "buckets" of increasing complexity:

| Term | Concept | Scaling Example (Game) | Scaling Example (Novel) |
| :--- | :--- | :--- | :--- |
| **Story** | The top-level container. | *The Chronicles of Aethelgard* | *The Glass Garden* |
| **Sequence** | An organizational bucket. | Act I: The Awakening | Chapter 1: The Discovery |
| **Scene** | The primary unit of work. | Level 1: The Ruined Village | Scene 1: The Basement |
| **Beat** | A discrete moment or idea. | Combat encounter, a plot twist | A specific character realization |

> **Key Insight**: A "Sequence" is just an organizational tool. It is not a mandatory container. You can have beats and scenes that exist independently of any sequence, allowing for pure discovery writing.

---

## 3. The Two Primary Workflows

The graph-based model enables two distinct modes of creation, catering to different cognitive styles.

### 3A. The Systems Approach (Top-Down)
Typical for game designers who think in terms of pacing, acts, and levels.
* **Workflow**: Define a Sequence $\rightarrow$ Populate with Scenes $\rightarrow$ Detail the Beats.
* **Goal**: Building an intentional structure that governs player experience.

### 3B. The Discovery Approach (Bottom-Up)
Typical for creative writers who think in terms of character moments and imagery.
* **Workflow**: Capture a Beat/Scene $\rightarrow$ Observe emerging patterns $\rightarrow$ Create a Sequence to group them.
* **Goal**: Capturing inspiration without the pressure of immediate organization.

---

## 4. Technical Foundations (The "How")

To make this philosophy possible, our architecture adheres to several critical technical constraints:

### 🔗 Relationship Logic
Unlike traditional systems where a child belongs to one parent, Anima Lab uses **explicit, bi-directional links**. 
* **[Implementation Note]**: We utilize junction tables (e.g., `scene_beat_links`) and many-to-many relationships to allow a single beat to be referenced by multiple scenes or sequences.

### 📝 The Outline as an Annotation
In Anima Lab, an outline is not the structure itself—it is an **attachment**.
* **[Implementation Note]**: Outlines are stored as optional metadata linked to a content item. This means you can write a scene first and add its "plan" later, or delete the plan without destroying the scene.

### 🔄 Seamless Re-organization
Because we use a graph, moving content does not cause "cascading failures."
* **[Implementation Note]**: Moving a scene from one sequence to another is a simple update of a foreign key or link. The internal beats and metadata move with it, untouched.

---

## 5. Summary: The Design Goal

The goal of Anima Lab is not to impose order on chaos, but to provide **scaffolding that appears only when invited.** By decoupling the *content* from its *structure*, we allow the tool to match the user's mental state—whether they are in a state of rapid-fire idea capture or deep, structured planning.

***
*Last Updated: [Date]*
