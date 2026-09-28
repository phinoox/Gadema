# 🚀 Performance & Infrastructure

This guide defines all performance optimization strategies, caching patterns, query guidelines, and infrastructure requirements for Anima Lab. It ensures the application maintains high performance under load while minimizing database queries and memory footprint.

**Target Audience**: Developers, DevOps Engineers, System Administrators  
**Coverage**: Query Optimization, Caching Strategy, Memory Management, API Response Time, Monitoring & Health Checks, Deployment

---

## 1️⃣ Database Query Optimization

### 1.1 Eager Loading with `Include()`

#### Architecture:
- Always use `.Include()` for relationship queries to prevent the N+1 problem.
- Use `.ThenInclude()` for nested relationships (e.g., `MetaInfo` $\rightarrow$ `MediaAttachments`).
- Never query navigation properties separately inside loops.

#### Implementation Pattern:
```csharp
// ✅ CORRECT - Eager loading to avoid N+1 queries with ProjectTask entity
public async Task<MetaInfo> GetMetaInfoWithFullDataAsync(Guid id, ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    return await _context.MetaInfos
        .Include(ci => ci.MediaAttachments)
        .Include(ci => ci.ContentTags)
            .ThenInclude(ct => ct.Tag)
        .Include(ci => ci.ProjectTasks)
            .ThenInclude(pt => pt.Comments)
        .FirstOrDefaultAsync(ci => ci.Id == id);
}

// ❌ INCORRECT - N+1 problem (Looping and querying separately)
foreach (var item in items)
{
    var tasks = await _context.ProjectTasks.Where(t => t.MetaInfoId == item.Id).ToListAsync();
}
```

### 1.2 Pagination Best Practices

#### Architecture:
- **Default page size**: 20 items (configurable up to 100).
- Always validate `pageSize` in controller actions.
- Use `Skip()` and `Take()` for efficient database-level pagination.

---

## 2️⃣ Caching Strategy

### 2.1 Redis Cache Configuration

#### Architecture:
- Use **Redis** for distributed caching in production environments.
- Implement **Domain-Aware Key Patterns** to avoid collisions (e.g., `tasks.{projectId}.{taskId}`).
- Set expiration based on content freshness:
    - **Draft Content**: 5 minutes TTL (high frequency of updates).
    - **Published Content**: 1 hour TTL (low frequency of updates).
    - **Static/System Data**: 24 hours TTL.

#### Implementation Pattern:
```csharp
// ✅ CORRECT - Redis caching with domain-aware key pattern and expiration
var cacheKey = $"tasks.{projectId}.{taskId}";
return await _redisCache.GetOrCreateAsync(cacheKey, async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1); // Published content TTL
    return await _context.ProjectTasks.FindAsync(projectId, taskId);
});
```

---

## 3️⃣ File Upload & Memory Management

### 3.1 Stream-Based Processing

#### Architecture:
- **Stream large files** directly to storage using `FileStream` instead of loading them into memory buffers.
- Validate file size **before** starting the upload process to prevent Out-Of-Memory (OOM) attacks.
- Use UUID-based filenames for security and collision avoidance.

#### Implementation Pattern:
```csharp
// ✅ CORRECT - Stream large file uploads
await using var fileStream = new FileStream(filePath, FileMode.Create);
await file.CopyToAsync(fileStream); // Memory-efficient streaming
```

### 3.2 Monitoring & Health Checks

#### Architecture:
- Monitor memory usage per instance (Target: `< 512MB`).
- Implement a `/health` endpoint to monitor:
    - Database connectivity.
    - Cache availability.
    - Current memory/request metrics.

---

## 4️⃣ Deployment & Infrastructure

### 4.1 Caddy Reverse Proxy Configuration

#### Architecture:
- Use **Caddy** for SSL auto-renewal and reverse proxying.
- Implement **Rate Limiting** at the proxy level to protect against DDoS and API abuse.

| Endpoint Type | Rate Limit | Rationale |
| :--- | :--- | :--- |
| General API (`/api/*`) | 100 req/min | Standard usage |
| Auth Endpoints (`/auth/*`) | 1000 req/hour | Protect against brute force |
| Export Endpoints (`/export/*`) | 5 req/min | High CPU/Resource intensity |

### 4.2 Production Deployment Checklist
- [ ] Set `ASPNETCORE_ENVIRONMENT` to `Production`.
- [ ] Configure PostgreSQL connection string with TLS.
- [ ] Enable Redis distributed caching.
- [ ] Verify SSL certificates via Caddy.
- [ ] Ensure rate limits are active on the proxy.

---

## 5️⃣ Performance KPIs

| Metric | Target | Measurement Method |
| :--- | :--- | :--- |
| **GET Response Time** | < 500ms | API Latency Monitoring |
| **POST/PUT Latency** | < 2s | API Latency Monitoring |
| **Database Query** | < 100ms | EF Core / SQL Profiler |
| **Memory Usage** | < 512MB | GC Counter / System Monitor |

***
*Last Updated: [Date]*
