# Access & Identity Domain

The **Access & Identity** domain defines how users are identified, how they belong to projects, and how automation interacts with the system. It is the foundation for the entire security model of GaDeMa.

## 🧬 Core Components

This domain manages three primary entities that work together to establish a secure, scoped environment:

| Entity | Role |
| :--- | :--- |
| [User](./user.md) | The central identity anchor representing an authenticated account. |
| [Project Member](./project-member.md) | The bridge defining a user's specific role and access within a project. |
| [Project Token](./project-token.md) | A secure, hashed key for automated or non-interactive access. |

## 🛡️ Authorization Model

GaDeMa utilizes a **Decoupled, Multi-Dimensional Authorization Engine**. Access is not just about *who* you are, but *where* you are acting.

### The Four-Gate Flow
Every request passes through four distinct security gates:

1.  **Visibility Gate**: Checks if the resource is marked as `IsPublic`.
2.  **Identity Gate**: Verifies the user's authentication via `IUserContext`.
3.  **Authorization Gate**: Uses the `IPermissionEngine` to check roles (e.g., Editor, Admin) within a specific project scope.
4.  **Execution Gate**: Once authorized, the service performs the requested operation.

---
*Part of the [Core Domain Hierarchy](./../hierarchy.md)*
