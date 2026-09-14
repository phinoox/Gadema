Since we are building a system that allows for **Scale-Invariant Storytelling**, a standard text summary isn't enough. You need to see the "shape" of the logic.

Here is a conceptual and visual overview of the GaDeMa architecture.

---

# 🌌 The GaDeMa Architecture: A Fractal Universe

The core philosophy is that **the world is fractal**. Whether you are looking at a single character or an entire galaxy, the rules for how they are identified, how they are searched, and how they are updated remain exactly the same.

## 1. The Structural Hierarchy (The "Bones")
This view shows how data is nested. A `ProjectSeries` is the ultimate container, but it doesn't "own" everything—it provides a **shared scope** for things to live in.

```text
[ PROJECT SERIES ] (The Universe / Shared Lore)
       │
       ├── [ Project: The Novel ] (Local Workspace)
       │      ├── Character A (Local - only exists here)
       │      └── Character B (Shared - part of the Series lore) ◄───┐
       │                                                             │
       ├── [ Project: The RPG ] (Local Workspace)                   │ (Shared via Scope)
       │      ├── Character C (Local - unique to this game)         │
       │      └── Character B (Shared - seen in the RPG too) ───────┘
       │
       └── [ Project: Action Game ] (Local Workspace)
              └── Character B (Shared - part of the universe's history)
```

---

## 2. The Entity Pattern (The "DNA")
Every single object in this system—whether it's a Series, a Project, or a Character—is constructed using the same **Anchor & Component** pattern. We never create "monolith" objects; we assemble them.

### The Formula: `Entity = Anchor (Identity) + Components (Domain Data)`

| Part | Role | Example (A Character) | Example (A Project) |
| :--- | :--- | :--- | :--- |
| **The Anchor** | **The Soul**: Defines *what* it is. Provides the identity used for searching and syncing. | `MetaInfo` (Name, Slug, Status) | `ProjectMetaInfo` (Title, Slug, Status) |
| **The Components**| **The Body**: Defines *how* it behaves in its specific medium. | `CombatStats`, `Backstory`, `Inventory` | `Genre`, `Tone`, `Audience` |

**Crucial Rule:** The Anchor is the "Source of Truth." If you change the Name in the Anchor, the name changes everywhere.

---

## 3. The Operational Patterns (The "Flow")
This is how data moves through the system via our two main engines.

### A. The Identity Sync Engine (Writing)
Instead of many small updates, we use **Strategy-based Synchronization**. This ensures that when an identity changes, the entire "identity footprint" stays consistent.

```text
[ User Input ] ──▶ [ Update DTO ] ──▶ [ Identity Sync Strategy ] ──▶ [ Atomic DB Transaction ]
                                              │
                                     (Calculates Deltas for:
                                      Title, Slug, Status, Tags)
```

### B. The Search Orchestration Engine (Reading)
We don't have a "Search Service" that knows everything. We have an **Orchestrator** that asks many specialized **Providers** for help.

```text
[ User Query: "Aragorn" ]
          │
          ▼
[ Search Orchestrator ] ◀─── (Asks in parallel) ───┐
          │                                        │
          ├─▶ [ Project Provider ] ───────────────┤
          ├─▶ [ Character Provider ] ─────────────┤ ──▶ [ Merged & Paginated Results ]
          └─▶ [ Location Provider ] ──────────────┘      (Standardized SearchHitDto)
```

---

## 4. The Authorization Logic (The "Shield")
Because we have a hierarchy, access is determined by **Scope**. This prevents users from accidentally editing the "Core Lore" of a series when they only have permission to edit their own local project.

| If the user owns... | They can Edit... | They can Read... |
| :--- | :--- | :--- |
| **The Project** | Local Assets (`ProjectId` matches) | Local + Shared Assets in the Series |
| **The Series** | All Shared Assets (`SeriesId` matches) | The entire Universe |
| **Nothing** | Nothing | Only their own local assets (if any) |

---

### Summary of the Vision
*   **Identity is decoupled from Domain:** You can change a character's name without touching their combat stats.
*   **Scope is explicit:** An object knows if it belongs to a Project or a Series.
*   **Discovery is unified:** Searching "The Universe" feels the same as searching "A Book."
*   **Scaling is seamless:** You can add new media types (e.g., a "Movie Project") just by adding new `Components`, without ever changing the core architecture.