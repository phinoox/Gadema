# GaDeMa — ADHD-Friendly CMS Design Summary

---

## 🧠 Core Philosophy

> **Structure should serve the writer, not constrain them.**

The system matches how ADHD brains actually work: non-linearly, associatively, and in bursts of focus followed by decision paralysis. Every design decision reduces friction between "I have an idea" and "that idea is safely stored."

---

## 📐 Core Architecture

```
┌─────────────────────────────────────────────────────┐
│                    GaDeMa CMS                        │
├─────────────────────────────────────────────────────┤
│  ┌──────────────┬────────────────────┬──────────┐   │
│  │  Content     │  Relationships     │  Meta-    │   │
│  │  Items       │  (many-to-many)    │  Data     │   │
│  │              │                    │           │   │
│  ├──────────────┼────────────────────┤──────────┤   │
│  │ Story →      │ • Scenes link to   │• Version- │   │
│  │ Sequences    │   Beats (M2M)      │  aware    │   │
│  │              │ • Characters       │  history  │   │
│  │              │   linked via       │           │   │
│  │              │   CharacterDetails  │          │   │
│  │              │   (FK-as-PK pattern)│          │   │
│  └──────────────┴────────────────────┴──────────┘   │
│                          │                           │
│                        ┌─┴─┐                         │
│                        │📷 │ ← Only manual snapshots │
│                        │💾 │ ← Autosave = fail-safe  │
│                        └───┘                         │
└─────────────────────────────────────────────────────┘
```

**Key design decisions:**

| Entity | PK Strategy | Why |
|--------|-------------|-----|
| `ProjectTaskComments` | FK (`TaskId`) as PK column | Eliminates nullable primary keys; row = comment |
| `ContentTags` | Self-composite key (`Id`) + FKs inline | No separate junction table needed |
| `CharacterBackground` | FK (`MetaInfoId`) as PK column | Direct link to character, no intermediate ID |

---

## 🎨 UI/UX Design Decisions

### 1. Split Workspace with Ghost Preview

```
┌──────────────┬─────────────────────────┐
│              │   OVERVIEW (collapsible)│
│  FOCUS MODE  ├─────────────────────────┤
│  Write       │ ┌─────────────────────┐│
│  here        │ │ Where to go next?  ││
│              │ │ • Quick wins: [ ]   ││
│ ───────────  │ │ • Connect X → Y     ││
│  Timeline    │ │ • Review outline    ││
│  (collapsible)│ └─────────────────────┘│
│  • Scene 1.1 ▼│                         │
│  • Scene 1.2 ◄┤  [Dismiss]   [Keep showing]│
│  • Scene 1.3 ◄└─────────────────────────┘│
└──────────────┴───────────────────────────┘
```

- **Ghost preview:** Small thumbnail of related elements visible but collapsible
- **Toggle with keyboard shortcut** — no mouse hunting needed
- Overview stays visible when collapsed, never hidden permanently

---

### 2. Two-Tier Save System (The Core Ritual)

#### Minor Save (Background)
```
[Save] → "✓ Saved in 0.4s • +127 words" → auto-dismisses after 3s
```
- **Overwrites** the last autosave silently
- **No version number shown.** Never says "v5."
- **No history entry created.** Invisible to version timeline.
- Purpose: *Safety only* — failsafe if you forget or crash.

#### Snapshot Save (Intentional)
```
[📸 Save Snapshot] → "✓ Saved as Milestone #12" → comment field appears
```
- **Creates a real version** in the timeline
- **Requires one-line comment** — forces micro-reflection
- **Suggestions appear ONLY after this action**
- Purpose: *Commitment & reflection* — marking a boundary you chose

#### Autosave (The Silent Guardian)
- Runs every 30–60 seconds in background
- Creates `.autosaved` versions internally, never visible to the user
- If tab crashes or browser closes → reopens with last autosaved state immediately
- **Never contributes** to versioned snapshot history

---

### 3. Version History as a Visual Timeline (Not a List)

```
[←] v1 ────► [v2] ─────► [v3] ◄──── [v4]
     (Jan 8)       (Jan 10)   (Jan 12)    (Jan 14)
              │          │         │        │
           "Intro"   "Added door motif" "Blocked on X research"
```

- **Horizontal scroll** — not a dropdown or modal
- Clicking any version jumps back to *that exact state*
- Comment appears as hover tooltip (not a full modal)
- Relative labels (`prev`, `next`) make navigation intuitive

---

### 4. "Where To Go From Here?" Panel

Appears **only after** a Snapshot Save. Dismissible at any time.

```
┌─────────────────────────────────────────────┐
│  ✨ Suggestions for what's next             │
│                                            │
│  ⚡ Quick wins (no new research):           │
│     [ ] Add a character emotion tag         │
│     [ ] Tag this scene with "tension"       │
│                                             │
│  🔍 Related items to review:                │
│     • The door motif appears in Scene 7    │
│       → Should we resolve it?              │
│     • CharacterBackground for Protagonist  │
│       is outdated (last edited 3 weeks ago)│
└─────────────────────────────────────────────┘

[Dismiss until next save]   [Keep showing me]
```

**Key rules:**
- Suggestions are **tied to the version** — if you undo to v2, suggestions disappear
- Can be dismissed without guilt ("I'll look at this on v4")
- No forms or multi-step setup required — single click to act

---

### 5. Connection Threads (Visual, Not Forced)

```
[Scene: The Confrontation] 
         │
    ┌────┴────┐
    │         │
[Beat]   [Beat]
  ⚡        ✨
   \      /
    \    /
[Outstanding beat in another scene that references this] → "This might connect here"
```

- Threads **pulse** when an unresolved link exists (a beat referenced elsewhere but not yet written)
- Dragging a thread between two elements creates the relationship — no forms
- If you don't know where something belongs? Leave it floating. The system suggests connections over time as you write more.

---

### 6. Idea Dump Zone (Non-Linear Capture)

```
┌─────────────────────────┐
│  IDEA DUMP (auto-saved) │ ← Always visible, never blocking
├─────────────────────────┤
│ • What if the weapon    │
│   glows when someone is lying?          │
│                           │
│ • The red thread motif   │
│   appears in Scene 7... │
│                           │
│ [Drag a beat here]       │ ← Drag-and-drop into existing scene
│ [Attach to existing scene]│
└─────────────────────────┘
```

- Ideas are saved instantly (no "write them down" friction)
- Can drag any idea into an existing scene later — no commitment required
- System gently suggests: *"You wrote about X in Scene 3. Do you want to connect this?"*

---

### 7. Time-Based Scaffolding Modes

| Mode | What It Hides | When To Use |
|------|---------------|-------------|
| **25-min sprint** | Everything except current scene + timer | When writer's block hits — reduce noise |
| **Freeform hour** | All structure; no tags, no beats required | First draft, discovery writing |
| **Retroactive planning** | Writer writes first → AI asks: *"What beats did you accidentally create?"* | After the fact structuring |

---

## 🧩 The Complete Flow (End-to-End)

```
┌─────────────────────────────────────────────────────┐
│  PHASE 1: FLOW STATE                                │
│  ───────────────────                                │
│  • Clean canvas — no suggestions visible            │
│  • Autosave happening silently in background        │
│  • Idea Dump open for capture (but non-intrusive)   │
│  • No decisions to make, no history to navigate     │
└───────────────┬─────────────────────────────────────┘
                ▼ [User stops typing / takes a break]
┌─────────────────────────────────────────────────────┐
│  PHASE 2: COMMIT RITUAL                             │
│  ───────────────────                                │
│  • Click [📸 Save Snapshot]                         │
│  • One-line comment appears (required)              │
│  • Version enters the timeline                      │
│  • Suggestions panel fades in *after* save          │
└───────────────┬─────────────────────────────────────┘
                ▼ [User chooses next action]
┌─────────────────────────────────────────────────────┐
│  PHASE 3: DECISION POINT                            │
│  ───────────────────                                │
│  • Click "Dismiss Suggestions" → back to flow       │
│  • Click a suggestion → it becomes the next task   │
│  • Or open a new draft without closing this one     │
└───────────────┬─────────────────────────────────────┘
                ▼ [User continues or dismisses]
┌─────────────────────────────────────────────────────┐
│  PHASE 4: BACK TO FLOW                              │
│  ───────────────────                                │
│  • Suggestions can be dismissed                     │
│  • Overview mode collapses suggestions               │
│  • User returns to clean writing space              │
└─────────────────────────────────────────────────────┘

This loop is the core of the ADHD-friendly workflow:
Flow → Commit → Decide → Flow → ...
```

---

## 🚫 What We Explicitly Avoid (Anti-Patterns)

| Pattern | Why It Fails for ADHD | Our Alternative |
|---------|----------------------|-----------------|
| Auto-save prompts ("You haven't saved!") | Creates anxiety, feels like nagging | Silent autosave; only snapshot creates visible history |
| Mandatory outlines before writing | Blocks momentum entirely | Outlines are optional attachments on any layer |
| Multi-step versioning dialogs | Friction kills the completion feeling | One-click snapshot → one-line comment → done |
| Linear tree navigation (Scene 1.1 → 1.2 → 1.3) | Doesn't match how ADHD brains associate ideas | Graph-based linking: drag threads between any elements |
| Pop-ups and modals for suggestions | Breaks flow, feels intrusive | Suggestions appear only after intentional save; dismissible at any time |

---

## 🧭 Summary of Core Principles

1. **Progressive disclosure** — Hide complexity until the user is ready to engage with it (save → then show suggestions)
2. **Ritual over friction** — Turn "I should have saved" into a satisfying completion ritual that feels like progress, not work
3. **Capture > structure** — Ideas are captured first; organization comes later as an optional layer on top
4. **Visual timeline over lists** — Temporal relationships are shown spatially (horizontal scroll), not hierarchically (dropdowns)
5. **Suggestions are suggestions** — Never required, never sticky, always dismissible with zero guilt

---

This is the complete design system for GaDeMa as an ADHD-friendly CMS. It's built on the insight that the tool shouldn't *fix* the writer's thinking process — it should match it and provide scaffolding only when explicitly invited to do so.