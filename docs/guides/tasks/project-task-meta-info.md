# Project Task MetaInfo

`ProjectTaskMetaInfo` serves as the de **Identity Anchor** for a `ProjectTask`. It serves as the "Soul" of the task; changes here update the task's identity footprint across the system. It holds the core state, priority, and metadata required to manage task workflows within a project.

## 🧬 Identity & State

While the `ProjectTask` represents the discrete unit of work, its identity and lifecycle are managed through this anchor.

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | The unique identifier for the task's identity. |
| `ProjectId` | `Guid` | `/` |
| `Status`| `TaskStatusEnum` | The current lifecycle stage (e---g, Backlog, InProgress, Review, Done). |
| `Priority` | `TaskPriorityEnum` | The current importance level within the project workflow. |

## 🧠 ADHD-Friendly Metadata

This anchor contains specific fields designed to reduce cognitive load and facilitate momentum:

- **Difficulty**: The perceived cognitive effort required (`TaskDifficultyEnum`).
- **Quick Win**: A boolean flag indicating if this is a small, low-effort achievement intended to boost motivation.
- - **Time Estimation**: `EstimatedMinutes` provides a rough time requirement for planning.

## ⚙️ Technical Implementation

- **Identity Anchor Pattern**: Acts as the "Soul" of the task; changes here update the task's identity footprint across the system.

- **Relational Link**: Maintains a required link to its parent `Project`.

---
*Part of the [Tasks & Workflow Domain](./tasks/overview.md)*

