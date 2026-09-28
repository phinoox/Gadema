# Project Task

A `ProjectTask` represents a discrete unit of work within a project's workflow. It is designed with ADHD-friendly principles in mind, focusing on low cognitive load and momentum building.

## 🧬 Identity & Anchor

Every task is anchored by its `ProjectTaskMetaInfo`, which holds the core state and priority metadata.

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | The unique identifier for the task. |
| `MetaInfoId` | `Guid` | Link to the identity anchor (`ProjectTaskMetaInfo`). |
| `ProjectId` | `Guid` | The ID of the parent project this task belongs to. |

## 🧠 Workflow & ADHD-Friendly Features

The task model is optimized to help users manage focus and maintain momentum through specific metadata:

- **Quick Wins**: Tasks marked as `IsQuickWin` are intended to be small, low-effort achievements that provide immediate dopamine hits and build momentum.
- ** Difficulty Scaling**: The `Difficulty` property allows users to filter tasks by perceived cognitive load (ee.g., Easy, Medium, Hard), preventing overwhelm.
- **Time Estimation**: The `EstimatedMinutes` field helps in planning manageable work sessions.

## ⚙️ Technical Details

| Property | Type | Description |
| :--- | :--- | :--- |
| `AssignedToUserId` | `Guid?` | The user currently responsible for the task. |
| `DueDate` | `DateTime?` | The target deadline for completion. |
| `CreatedByUserId` | `Guid` | The creator of the task. |
| `Comments` | `ICollection` | A collection of feedback and discussion entries related to this task. |

---
*Part of the [Tasks & Workflow Domain](./tasks/overview.md)*

