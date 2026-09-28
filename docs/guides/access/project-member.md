# Project Member

The `ProjectMember` entity defines the relationship between a `User` and a `Project`. It acts as the authorization bridge, determining what actions a user can perform within a specific project's scope.

## 🛡️ Access & Roles

Access is managed through a scoped role system. Instead of global permissions, a user's capabilities are determined by their membership in a particular project.

| Role | Description |
| :--- | :--- |
| `Owner` | Full administrative control and primary ownership of the project. |
| `Admin` | High-level management privileges within the project scope. |
| `Editor` | Capable of creating, editing, and managing content/tasks. |
| `Reviewer` | Focused on reviewing progress and providing feedback. |
| `Viewer` | Read-only access to all project contents. |

## 🧬 Membership Details

A membership record links a user's identity to a project's workspace via the following data:

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | Unique identifier for this membership instance. |
| `ProjectId` | `Guid` | The ID of the project being joined. |
| `UserId` | `Guid` | The ID of the user joining the project. |
| `Role` | `ProjectMemberRoleEnum` | The assigned permission level within this scope. |
| `JoinedAt` | `DateTime` | Timestamp of when the membership was established. |

## ⚙️ Technical implementation

- **Scope Binding**: This entity is the "Muscle" in the Four-Gate Flow, providing the context used by the `PermissionEngine` to resolve whether a user has the required role for a specific action.
- **Relational Integrity**: Maintains strict links to both the `Project` and the `User` via Required navigation properties.

---
*Part of the [Access & Identity Domain](./access/overview.md)*
