// =============================================================================
// Gadema.Core.Dtos.Base.Projects — All Project Token Related DTOs
// 
// Conventions:
// - No nested DTOs (everything flat at top level)
// - CreateData / UpdateData / Response patterns inherited from base types
// - ProjectId always derived from route, never in body
// =============================================================================

using System;

using Gadema.Core.Dtos.Response;

namespace Gadema.Core.Dtos.Base.Projects;

/// <summary>
/// Enumeration of API token types.
/// </summary>
public enum ProjectTokenType
{
    ReadOnly = 0,
    ReadWrite = 1,
    Admin = 2,
    WriteOnly = 3
}

// ========================================================================
// CREATE DTO — No nested CreateData, all fields flat at top level
// ========================================================================

/// <summary>
/// Request body for creating a new API token.
/// </summary>
public class ProjectTokenCreateDto : UpdateRequestDto
{
    /// <summary>
    /// Type of access the token should have.
    /// - ReadOnly: Can only read content, cannot modify or delete.
    /// - ReadWrite: Full CRUD access to project entities.
    /// - Admin: Elevated permissions for administrative actions.
    /// - WriteOnly: Can create/update but cannot read existing data (use case: write-only integrations).
    /// </summary>
    public int TokenType { get; set; } = (int)ProjectTokenType.ReadWrite;

    /// <summary>
    /// Expiration date for the token. Defaults to 1 year from creation if not specified.
    /// Set to null for non-expiring tokens (not recommended).
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// If true, the generated token value will be returned in the response.
    /// If false, only metadata is returned (token string hidden for security).
    /// Default: false (security best practice).
    /// </summary>
    public bool RevealToken { get; set; } = false;

    // Inherited from UpdateRequestDto:
    //   Guid Id { get; set; } — used for upsert patterns if needed
}

// ========================================================================
// UPDATE DTO — Partial update pattern (all fields nullable)
// ========================================================================

/// <summary>
/// Request body for updating an existing API token.
/// Only provided fields will be updated (partial update pattern).
/// </summary>
public class ProjectTokenUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// New token type. If null, the current type is preserved.
    /// </summary>
    public int? TokenType { get; set; }

    /// <summary>
    /// New expiration date. If null, the current expiration is preserved.
    /// Set to a past DateTime to effectively revoke the token without changing IsRevoked flag.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    // Note: Revoke action uses DELETE endpoint, not PUT.
}

// ========================================================================
// RESPONSE DTO — Flat structure with denormalized fields for convenience
// ========================================================================

/// <summary>
/// Response DTO for a single API token detail response.
/// Includes both the raw token (if requested) and metadata.
/// </summary>
public class ProjectTokenResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The actual token string. Only included if RevealToken=true was set during creation.
    /// If false, this field will be null for security reasons.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Type of access granted by the token.
    /// 0 = ReadOnly, 1 = ReadWrite, 2 = Admin, 3 = WriteOnly
    /// </summary>
    public int TokenType { get; set; }

    /// <summary>
    /// Whether this token has been revoked and is no longer valid.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// The date/time when the token expires (or null for non-expiring).
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    // Inherited from MetaInfoResponseBaseDto:
    //   Guid Id, Guid ProjectId, string Title, ContentStatusEnum Status, bool IsPublic,
    //   DateTime CreatedAt, DateTime LastModifiedAt
}

// ========================================================================
// LIST RESPONSE DTO — Paginated list of all project tokens
// ========================================================================

/// <summary>
/// Response DTO for listing API tokens in a project.
/// </summary>
public class ProjectTokenListResponseDto : ListResponseDto<ProjectTokenResponseDto>
{
    // Inherits: Items, TotalCount, Page, PageSize, TotalPages from base class
}

// ========================================================================
// USAGE EXAMPLES — How to use these DTOs in controllers/services
// ========================================================================

/*
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  POST /api/v1/projects/{projectId:guid}/tokens                  │
 * └─────────────────────────────────────────────────────────────────┘
 * Request Body:
 * {
 *   "tokenType": 1,               // ProjectTokenType.ReadWrite (default)
 *   "expirationDate": null,       // null = non-expiring, or set a date
 *   "revealToken": false          // false for security, true to see the token
 * }
 * 
 * Response:
 * {
 *   "success": true,
 *   "data": {
 *     "id": "a1b2c3d4-...",
 *     "projectId": "{projectId}",
 *     "title": "API Token",          // from MetaInfo.Title (auto-generated)
 *     "status": 0,                   // Draft/InProgress/Published
 *     "isPublic": false,
 *     "createdAt": "2025-01-15T10:30:00Z",
 *     "lastModifiedAt": "2025-01-15T10:30:00Z",
 *     "tokenType": 1,                // ReadWrite
 *     "token": null,                 // null because revealToken=false (default)
 *     "isRevoked": false,
 *     "expirationDate": null         // non-expiring
 *   }
 * }
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  GET /api/v1/projects/{projectId:guid}/tokens                   │
 * └─────────────────────────────────────────────────────────────────┘
 * Response:
 * {
 *   "success": true,
 *   "data": {
 *     "items": [
 *       {
 *         "id": "a1b2c3d4-...",
 *         "projectId": "{projectId}",
 *         "title": "Production API Token",
 *         "status": 0,
 *         "isPublic": false,
 *         "createdAt": "2025-01-10T08:00:00Z",
 *         "lastModifiedAt": "2025-01-14T12:00:00Z",
 *         "tokenType": 1,
 *         "token": null,              // never revealed in list response
 *         "isRevoked": false,
 *         "expirationDate": "2026-01-10T08:00:00Z"
 *       },
 *       {
 *         "id": "e5f6g7h8-...",
 *         "projectId": "{projectId}",
 *         "title": "Staging Token",
 *         "status": 0,
 *         "isPublic": false,
 *         "createdAt": "2025-01-12T14:30:00Z",
 *         "lastModifiedAt": "2025-01-14T09:00:00Z",
 *         "tokenType": 2,             // Admin
 *         "token": null,
 *         "isRevoked": true,          // this one is revoked
 *         "expirationDate": null
 *       }
 *     ],
 *     "totalCount": 2,
 *     "page": 1,
 *     "pageSize": 50,
 *     "totalPages": 1
 *   }
 * }
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  POST /api/v1/projects/{projectId:guid}/tokens/revoke           │
 * └─────────────────────────────────────────────────────────────────┘
 * Note: Revoke is typically a DELETE on the token ID, not a separate endpoint.
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  PUT /api/v1/projects/{projectId:guid}/tokens/{tokenId:guid}   │
 * └─────────────────────────────────────────────────────────────────┘
 * Request Body (partial update — only provided fields change):
 * {
 *   "id": "{tokenId}",
 *   "expirationDate": "2026-01-15T10:30:00Z"  // Only this field is updated
 * }
 * 
 * Response:
 * {
 *   "success": true,
 *   "data": {
 *     "id": "{tokenId}",
 *     "projectId": "{projectId}",
 *     "title": "Production API Token",
 *     "status": 0,
 *     "isPublic": false,
 *     "createdAt": "2025-01-10T08:00:00Z",
 *     "lastModifiedAt": "2026-03-15T14:22:00Z",  // updated timestamp
 *     "tokenType": 1,
 *     "token": null,
 *     "isRevoked": false,
 *     "expirationDate": "2026-01-15T10:30:00Z"   // UPDATED
 *   }
 * }
 */

// ========================================================================
// HELPER ENUM EXTENSION (optional — for frontend/type safety)
// ========================================================================

public static class ProjectTokenTypeExtensions
{
    public static string ToDescription(this ProjectTokenType type) => type switch
    {
        ProjectTokenType.ReadOnly => "Read Only",
        ProjectTokenType.ReadWrite => "Read/Write",
        ProjectTokenType.Admin => "Admin (Elevated)",
        ProjectTokenType.WriteOnly => "Write Only",
        _ => $"Unknown ({(int)type})"
    };

    public static string ToDisplayName(this ProjectTokenType type) => type switch
    {
        ProjectTokenType.ReadOnly => "Read-Only Access",
        ProjectTokenType.ReadWrite => "Full Access (Read/Write)",
        ProjectTokenType.Admin => "Admin Access",
        ProjectTokenType.WriteOnly => "Write-Only Access",
        _ => $"Unknown {(int)type}"
    };
}