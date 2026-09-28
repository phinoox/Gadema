# Project Task Comment

A `ProjectTaskComment` represents a piece of user-generated feedback or discussion attached to a specific task. In the GaDeMa architecture, comments are treated as **Components**—ancillary data that is tethered to an Anchor (the `ProjectTask`).

## 🧬 Identity & Linkage

Comments exist within the context of a task and provide essential social/feedback layers to the workflow.

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | Unique identifier for the comment. |
| `ProjectTaskId` | `Guid` | The ID of the parent task this comment belongs to. |
| `CommentedByUserId` | `Guid` | The identity of the user who authored the comment. |

## 📝 Content & Lifecycle

Comments are designed to facilitate communication and documentation within a project's workflow.

| Property | Type | Description |
| :--- | :--- | :--- |
| `CommentText` | `string` | The actual content of the comment (supports Markdown/HTML). |
| `CreatedAt` | `DateTime` | Timestamp indicating when the comment was authored. |
| `TaskId` | `Guid` | A secondary identifier for internal linking and junction patterns. |

## ⚙️ Technical Implementation

- **Component Pattern**: Comments are not independent entities; they are "anonymous" until attached to a task via the `ProjectTaskId`.
- **Scope**: Access to comments is governed by the permissions of the parent `ProjectTask` and the user's role within that project.
- **Formatting**: Supports rich text (Markdown/HTML) for detailed feedback and context.

---
*Part of the [Tasks & Workflow Domain](./tasks/overview.md)*

