# 🔀 Hybrid Response Pattern Guidelines for GaDeMa v0.1

This document provides detailed implementation guidance on RAW vs WRAPPED response patterns across the application.

## Overview

The **Hybrid Response Pattern** is a key architectural decision in v2:

| Response Type | When to Use | Example |
| :--- | :--- | :--- |
| **RAW** | Simple data retrieval, list operations | `{ "data": [...] }` |
| **WRAPPED** | User-triggered confirmations, errors, side effects | `{ "success": true, "message": "...", "data": {...} }` |

---

## When to Use RAW Responses

### List Operations (GET /items)

```json
// ✅ CORRECT - Simple list retrieval
{
  "data": [
    {
      "id": "char-001",
      "title": "Geralt of Rivia"
    },
    {
      "id": "char-002",
      "title": "Ziggy Stardust"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalItems": 25,
    "totalPages": 3
  }
}
```

### Single Item Retrieval (GET /item/{id})

```json
// ✅ CORRECT - Simple data retrieval
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "slug": "geralt-of-rivia",
  "contentType": 1,
  "description": "...",
  "published": true,
  "viewMode": "Presentation"
}
```

### Creation Response (POST /items) - Simple Success

```json
// ✅ CORRECT - Simple creation without confirmation needed
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "slug": "geralt-of-rivia",
  "contentType": 1,
  "published": false,
  "version": 1,
  "status": "Draft"
}
```

---

## When to Use WRAPPED Responses

### User-Triggered Confirmations (PUT /items/{id})

```json
// ✅ CORRECT - User-triggered confirmation needed
{
  "success": true,
  "message": "Content item updated successfully",
  "data": {
    "id": "char-001",
    "title": "Geralt of Rivia",
    "description": "...updated...",
    "published": true,
    "version": 2
  }
}
```

### Delete Operations (DELETE /items/{id})

```json
// ✅ CORRECT - Confirmation with side effects
{
  "success": true,
  "message": "Content item has been deleted successfully",
  "data": null
}
```

### File Uploads (POST /upload)

```json
// ✅ CORRECT - Side effect (file stored), confirmation needed
{
  "success": true,
  "message": "Media file uploaded successfully",
  "data": {
    "id": "att-001",
    "filename": "character-concept.png",
    "contentType": "image/png",
    "storagePath": "/uploads/2024/03/char-concept.png",
    "uploadedAt": "2024-03-15T14:30:00Z"
  }
}
```

### Export Operations (POST /export/*)

```json
// ✅ CORRECT - Side effect (file generated), confirmation needed
{
  "success": true,
  "message": "JSON export generated successfully",
  "data": {
    "project": {
      "id": "proj-001",
      "title": "The Dragon's Crown"
    },
    "MetaInfos": [
      {
        "id": "char-001",
        "title": "Geralt",
        "published": true,
        "attributes": []
      }
    ],
    "version": "1.0"
  }
}
```

### Error Responses (All 4xx and 5xx)

```json
// ✅ CORRECT - Always wrapped for errors
{
  "success": false,
  "errors": ["Description cannot be empty", "Title is required"],
  "message": "Validation failed"
}

// ✅ CORRECT - Not found error (wrapped)
{
  "success": false,
  "message": "Resource not found",
  "errors": ["Character not found"]
}
```

---

## Controller Implementation Examples

### Example 1: Get Content Item with View Mode Separation

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetMetaInfoAsync(
    Guid id, 
    [FromQuery] ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    var MetaInfo = await _context.MetaInfos.FindAsync(id);
    
    if (MetaInfo == null)
        return NotFound(new 
        {
            success = false,
            errors = ["Content item not found"],
            message = "Resource not found"
        });  // ✅ WRAPPED for error
    
    // PrivateWriting mode: return full content + admin tools
    if (viewMode == ViewModeEnum.PrivateWriting)
    {
        return Ok(new 
        {
            id = MetaInfo.Id,
            title = MetaInfo.Title,
            description = MetaInfo.Description,
            shortDesc = MetaInfo.ShortDesc,
            published = MetaInfo.Published,
            viewMode = ViewModeEnum.PrivateWriting,
            version = MetaInfo.Version,
            status = MetaInfo.Status,
            createdAt = MetaInfo.CreatedAt,
            lastModifiedAt = MetaInfo.LastModifiedAt
        });  // ✅ RAW for simple data retrieval
    }
    
    // Presentation mode: return only published fields + clean layout
    return Ok(new 
    {
        id = MetaInfo.Id,
        title = MetaInfo.Title,
        description = MetaInfo.Description,
        slug = MetaInfo.Slug,
        seoTitle = $"@MetaInfo.Title - GaDeMa"  // SEO-friendly
    });  // ✅ RAW for simple data retrieval
}
```

### Example 2: Update Content Item with Confirmation

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateMetaInfoAsync(
    Guid id, 
    [FromBody] UpdateMetaInfoDto dto)
{
    try
    {
        var MetaInfo = await _context.MetaInfos.FindAsync(id);
        
        if (MetaInfo == null)
            return NotFound(new 
            {
                success = false,
                errors = ["Content item not found"],
                message = "Resource not found"
            });  // ✅ WRAPPED for error
        
        // Update fields
        MetaInfo.Description = dto.Description ?? "";
        MetaInfo.ShortDesc = dto.ShortDesc;
        MetaInfo.Published = dto.Published;
        MetaInfo.LastModifiedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        // Increment version number (polymorphic design)
        MetaInfo.Version++;
        
        return Ok(new 
        {
            success = true,  // ✅ WRAPPED for user-triggered confirmation
            message = "Content item updated successfully",
            data = new 
            {
                id = MetaInfo.Id,
                title = MetaInfo.Title,
                description = MetaInfo.Description,
                shortDesc = MetaInfo.ShortDesc,
                published = MetaInfo.Published,
                viewMode = MetaInfo.ViewMode,
                version = MetaInfo.Version,
                status = MetaInfo.Status
            }
        });  // ✅ WRAPPED for user-triggered confirmation
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating content item: {Id}", id);
        return StatusCode(500, new 
        {
            success = false,
            message = "An error occurred while updating content item",
            errors = [ex.Message]
        });  // ✅ WRAPPED for server error
    }
}
```

### Example 3: Delete Content Item with Confirmation

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteMetaInfoAsync(Guid id)
{
    try
    {
        var MetaInfo = await _context.MetaInfos.FindAsync(id);
        
        if (MetaInfo == null)
            return NotFound(new 
            {
                success = false,
                errors = ["Content item not found"],
                message = "Resource not found"
            });  // ✅ WRAPPED for error
        
        // Delete content item
        _context.MetaInfos.Remove(MetaInfo);
        
        await _context.SaveChangesAsync();
        
        return Ok(new 
        {
            success = true,  // ✅ WRAPPED for user-triggered confirmation
            message = "Content item has been deleted successfully",
            data = null  // No additional data needed for deletion
        });  // ✅ WRAPPED for user-triggered confirmation
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting content item: {Id}", id);
        return StatusCode(500, new 
        {
            success = false,
            message = "An error occurred while deleting content item",
            errors = [ex.Message]
        });  // ✅ WRAPPED for server error
    }
}
```

### Example 4: File Upload with Confirmation

```csharp
[HttpPost("content-items/{id}/media/upload")]
public async Task<IActionResult> UploadMediaFileAsync(Guid id, IFormFile file)
{
    try
    {
        // Validate MIME type BEFORE processing file
        var allowedMimeTypes = new[] 
        {
            "image/png", "image/jpeg", "application/pdf"
        };
        
        if (!allowedMimeTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new 
            {
                success = false,  // ✅ WRAPPED for validation error
                errors = ["Invalid file type. Only images and PDFs are allowed."],
                message = "Validation failed"
            });
        }
        
        // Validate file size BEFORE uploading (Max 100MB)
        const int MaxFileSize = 100 * 1024 * 1024;
        
        if (file.Length > MaxFileSize)
        {
            return BadRequest(new 
            {
                success = false,
                errors = [$"File too large. Maximum size: {MaxFileSize / 1024 / 1024}MB"],
                message = "Validation failed"
            });
        }
        
        // Generate secure filename and upload to storage
        var uniqueFilename = $"content-{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
        var sanitizedPath = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(sanitizedPath);
        
        var filePath = Path.Combine(sanitizedPath, uniqueFilename);
        
        // Stream the uploaded file directly to storage (memory-efficient)
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        
        return Ok(new 
        {
            success = true,  // ✅ WRAPPED for upload confirmation
            message = "Media file uploaded successfully",
            data = new 
            {
                id = Guid.NewGuid(),
                filename = uniqueFilename,
                contentType = file.ContentType,
                storagePath = filePath,
                uploadedAt = DateTime.UtcNow
            }
        });  // ✅ WRAPPED for upload confirmation (side effect)
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error uploading media file to content item: {Id}", id);
        return StatusCode(500, new 
        {
            success = false,
            message = "An error occurred while uploading media file",
            errors = [ex.Message]
        });  // ✅ WRAPPED for server error
    }
}
```

### Example 5: Export to JSON with Confirmation

```csharp
[HttpPost("projects/{id}/export/json")]
public async Task<IActionResult> ExportToJsonAsync(Guid id, [FromBody] ExportOptionsDto dto)
{
    try
    {
        // Query published content items for export
        var items = await _context.MetaInfos
            .Where(i => i.ProjectId == id && i.Published)
            .Include(i => i.MediaAttachments)
            .ToListAsync();
        
        // Generate JSON with QuestPDF
        var exportData = GenerateExportData(items, dto);
        
        return Ok(new 
        {
            success = true,  // ✅ WRAPPED for export confirmation (side effect)
            message = "JSON export generated successfully",
            data = new 
            {
                projectId = id,
                filename = $"export-{Guid.NewGuid()}.json",
                contentType = "application/json",
                downloadUrl = $"/exports/{filename}",
                exportAt = DateTime.UtcNow,
                sections = dto.Sections,
                includeWatermark = dto.IncludeWatermark
            }
        });  // ✅ WRAPPED for export confirmation (side effect)
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error exporting project to JSON: {Id}", id);
        return StatusCode(500, new 
        {
            success = false,
            message = "An error occurred while generating JSON export",
            errors = [ex.Message]
        });  // ✅ WRAPPED for server error
    }
}
```

---

## Decision Tree: RAW vs WRAPPED

```mermaid
graph TD
    A[Response Type?] --> B{Side Effect?}
    B -->|Yes| C[Use WRAPPED]
    B -->|No| D{User Triggered?}
    D -->|Yes| C
    D -->|No| E[Use RAW]
    
    C --> F[{"success": true/false, "message": "...", "data": {...}}]
    E --> G[{"data": {...}, "pagination": {...}}]
```

---

## Summary: Response Pattern Checklist

| Scenario | Use RAW or WRAPPED? | Example |
| :--- | :--- | :--- |
| **List operations** (GET /items) | ✅ RAW | `{ "data": [...] }` |
| **Single item retrieval** (GET /item/{id}) | ✅ RAW | `{ "id": "...", "title": "..." }` |
| **Simple creation** (POST /items) | ✅ RAW | `{ "id": "...", "title": "..." }` |
| **Update with confirmation** (PUT /items/{id}) | ❓ Context-dependent | See examples above |
| **Delete operations** (DELETE /items/{id}) | ✅ WRAPPED | `{ "success": true, "message": "..." }` |
| **File uploads** (POST /upload) | ✅ WRAPPED | `{ "success": true, "data": { "filename": "..." } }` |
| **Export operations** (POST /export/*) | ✅ WRAPPED | `{ "success": true, "message": "...", "data": {...} }` |
| **Error responses** (All 4xx/5xx) | ✅ WRAPPED | `{ "success": false, "errors": [...] }` |

---

## Final Reminder

**Follow these response pattern guidelines to maintain consistency across the API!**

- ✅ Use **RAW** for simple data retrieval and list operations
- ✅ Use **WRAPPED** for user-triggered confirmations and side effects (uploads, exports, deletions)
- ✅ Always wrap error responses in WRAPPED format
```
