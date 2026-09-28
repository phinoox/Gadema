# Project Token

A `ProjectToken` represents an automated access key for a specific project. It allows external systems (such as CI/CD pipelines or automation scripts) to interact with the GaDeMa API without requiring a user's interactive session.

## 🔐 Security & Authentication

To ensure security, tokens are never stored in plain text. Instead, they follow a strict hashing and verification pattern:

- **Storage**: Only the `TokenHash` (a secure SHA256 + salt) is persisted in the database.
- **Verification**: During an API request, the incoming token is hashed and compared against the stored hash to validate authenticity.

## 📊 Token Capabilities & Quotas

Tokens can be configured with specific constraints to prevent abuse and manage resource usage:

| Property | Type | Description |
| :--- | :--- | :--- |
| `TokenName` | `string` | A descriptive label (e.g., "CI/CD Pipeline"). |
| `MaxRequests` | `int` | The total request quota allowed (0 = unlimited). |
| `CurrentUsage` | `int` | The number of requests already consumed by this token. |
| `ExpiresAt` | `DateTime?` | An optional expiration timestamp for temporary access. |
| `PermissionsJson` | `string` | A JSON-encoded array defining granular permissions granted to the token. |

## ⚙️ Technical Implementation

- **Scope**: Every token is anchored to a specific `ProjectId`.
- **Usage Tracking**: The system monitors usage via the `CurrentUsage` property, allowing for real-time quota enforcement.
- **Automation Ready**: Designed specifically for non-interactive environments where standard OAuth flows are not feasible.

---
*Part of the [Access & Identity Domain](./access/overview.md)*
