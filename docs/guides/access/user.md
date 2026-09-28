# User

The `User` entity represents a unique account within the GaDeMa system. It serves as the primary identity used by the **Identity Gate** to establish who is interacting with the application.

## 🧬 Identity & Role

A user's presence in the system is defined by their authentication credentials and their ability to hold roles across different organizational scopes (Teams and Projects).

| Property | Type | Description |
| :--- | :--- | :--- |
| `Id` | `Guid` | The unique identifier used for all authorization checks. |
| `UserName` | `string` | Unique identifier for login and identification. |
| `Email` | `string` | Verified contact address. |
| `DisplayName` | `string?` | The human-readable name shown in the UI. |
| `Provider` | `UserAuthProviderEnum` | Defines the primary authentication method (e.g., Password, Google). |

## 🛡️ Security & Access

The user identity is the foundation for the **Four-Gate Flow**:

1.  **Identity Gate**: The system uses the `User.Id` via `IUserContext` to confirm a session exists.
2.  **Authorization Gate**: Once identified, the user's permissions are evaluated against specific resources (e.g., `Project`, `Scene`) based on their memberships in **Teams** or **Projects**.

## ⚙️ Technical Details

- **Authentication Providers**: Supports standard password-based login and external providers like Google OAuth via `ProviderLinks`.
- **Account Status**: Managed through the `IsActive` flag, which can be used to suspend access without deleting the identity.
- **Lifecycle**: Tracks account creation (`CreatedAt`) and recent activity (`LastLogin`).

---
*Part of the [Auth & Team Domain](./access/overview.md)*

