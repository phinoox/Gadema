# Writing Domain Overview

The **Writing** domain provides the tools for both structured narrative (like game design) and fluid discovery-based writing (like novel drafting). It is built on a "Graph, Not Tree" philosophy to support non-linear thinking.

## 🧬 Core Concept: Graph-Based Storytelling

Unlike traditional hierarchical systems that force a rigid parent-child structure, the GaDeMa writing engine treats all narrative elements as nodes in a flexible graph. This allows writers to move between high-level planning and granular drafting without being constrained by structural rules.

### The Hierarchy of Intent

| Level | Entity | Role |
| :--- | :--- | :--- |
| **Container** | `Sequence` | An organizational "bucket" (e.g., Act, Chapter, or Theme Group). |
| **Unit of Work** | `Scene` | A discrete chunk of narrative (the primary entry point for most writers). |
| **Moment** | `Storybeat` | Small, meaningful narrative turning points that drive the story forward. |

---

## 🧠 Design Philosophy: Structure as a Choice

The system is designed to accommodate two distinct creative modes:

### 1. The Sequence-First Approach (Game/Systems Thinking)
Used by designers who think in pacing and structure. They define **Sequences** first, then populate them with **Scenes** and **Beats**. 
*Example: A Game Designer planning "Act I" $\rightarrow$ "Level 1" $\rightarrow$ "Tutorial Encounter".*

### 2. The Scene-First Approach (Discovery Writing)
Used by authors who write non-linearly. They create **Scenes** as they come to them, without a predefined structure. Later, they use **Sequences** to group these scenes into chapters or thematic clusters.
*Example: A Novelist writing "The Discovery Scene" and "The Climax" separately, then grouping them into a chapter later.*

---

## 🛠️ Key Features for Narrative Control

- **Optional Outlines**: Outlines are attached as flexible annotations to any level (Story, Sequence, or Scene). They provide scaffolding without being mandatory.
- **Non-Linear Links**: Entities can be linked via bi-directional relationships (e e.g., a Beat can reference multiple Scenes) without breaking the hierarchy.
- **View Mode Separation**: 
  - `PrivateWriting`: For the messy, iterative drafting process.
  - `Presentation`: For viewing the polished, structural narrative for review or export.

## ⚙️ Technical implementation

| Concept | Implementation Detail |
| :--- | :--- | :--- |
| **Structural Integrity** | Uses a graph structure where relationships are explicit links rather than rigid inheritance. |
| **Collapsible Hierarchy** | Supports hierarchical view (Sequence $\rightarrow$ Scene) for high-level overview without losing detail. |
| **The Nexus (Scene)** | The `Scene` acts as the intersection between static narrative structure and dynamic character state. |

---
*Part of the [Core Domain Hierarchy](./../hierarchy.md)*

