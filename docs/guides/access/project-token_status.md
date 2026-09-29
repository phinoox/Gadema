# 🔐 Status: Project Token

This document provides a validation report comparing the automated access model outlined in `project-token.md` against the current technical implementation.

## 🔍 Comparison Summary

| Concept | Implementation Status | Findings & Discrepancies |
| :--- | :--- | :--- |
| **Security (Hashing)** | ✅ Match | The `ProjectToken` model includes a `TokenHash` property, supporting the documented SHA256 + salt pattern. |
| **Capabilities & Quotas** | ✅ Match | The implementation of `MaxRequests`, `CurrentUsage`, and `ExpiresAt` in the `ProjectToken` model directly supports the described quota enforcement mechanism. |
| **Granular Permissions** | ✅ Match | The presence of a `PermissionsJson` property in the `ProjectToken` model confirms the support for fine-grained, JSON-encoded permission sets as described. |
| **Scope (Project Binding)** | ✅ Match | The `ProjectId` property on the token ensures every key is anchored to a specific project scope. |

## 🛠️ Recommendations

*   **Verify Usage Tracking Logic**: Ensure that the `ProjectTokenService` actively increments `CurrentUsage` during each valid request and enforces the `MaxRequests` limit in real-time.
*   **Validate JSON Schema**: Since permissions are stored as a JSON string, establish a strict schema for `PermissionsJson` to ensure consistency across different token types and prevent parsing errors.
*   **Automated Testing**: Implement unit tests specifically targeting the "Quota Exhaustion" scenario (reaching `MaxRequests`) and the "Expiration" scenario (`ExpiresAt`).

## 📋 Developer Guide Audit

| Step | Status | Note |
| :--- | :--- | :--- |
| 1. Security Model | ✅ Match | Hashing and verification properties are present in models. |
| 2. Property Mapping | ✅ Match | All documented properties (`MaxRequests`, `CurrentUsage`, etc.) exist in the code. |
| 3. Scope Implementation | ✅ Match | Project-based scoping is built into the entity model. |
