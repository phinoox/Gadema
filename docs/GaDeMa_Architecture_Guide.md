# 📐 GaDeMa Architecture Guide

> **A visual guide to understanding how GaDeMa is built.**  
> This document complements `ARCHITECTURE.md` with diagrams, UI mockups, and flow explanations.

---

## Table of Contents

1. [System Overview](#system-overview) — Big picture architecture
2. [Layer Architecture](#layer-architecture) — How layers connect
3. [Data Flow Diagrams](#data-flow-diagrams) — Request/response flows
4. [UI Component Hierarchy](#ui-component-hierarchy) — Frontend structure
5. [Database Schema Overview](#database-schema-overview) — Entity relationships
6. [Security Architecture](#security-architecture) — Auth & authorization
7. [Deployment Architecture](#deployment-architecture) — Infrastructure

---

## System Overview

```mermaid
graph TB
    subgraph Client ["Client Layer"]
        WebApp[Blazor Server App]
        SignalR[SignalR WebSocket<br/>Real-time updates]
    end
    
    subgraph API ["API Gateway / HTTP Layer"]
        Gateway[Gadema.Api<br/>ASP.NET Core 8+]
        CORS[CORS Middleware]
        JWTAuth[JWT Auth Middleware<br/>HS256 Token Validation]
        RateLimit[Rate Limiter<br/>100 req/min per IP]
    end
    
    subgraph Business ["Business Logic Layer"]
        Services[Services Layer<br/>IUserService, IContentService,<br/>IProjectService, etc.]
        DTOs[DTO Mapping & Validation<br/>DataAnnotations + Fluent API]
        Policies[Policies: OwnProject,<br/>AdminOnly, TeamLead]
    end
    
    subgraph Data ["Data Access Layer"]
        EFCore[EF Core DbContext<br/>Code-First Migrations]
        Configs[Fluent API Configs<br/>per Entity / per Domain]
        Postgres[(PostgreSQL / SQLite)]
    end
    
    subgraph Core ["Core Domain (Pure .NET Standard)"]
        Models[Domain Models]
        DTOsCore[DTO Contracts]
        Enums[Enum Definitions]
        Interfaces[Interface Contracts]
    end
    
    WebApp -->|REST API Calls| Gateway
    CORS --> Gateway
    JWTAuth --> Gateway
    RateLimit --> Gateway
    Gateway --> Services
    Services --> DTOs
    Policies --> Services
    Services --> EFCore
    EFCore --> Configs
    Configs --> Postgres
    Core -.-> Models
    Core -.-> DTOsCore
    Core -.-> Enums
    Core -.-> Interfaces
    
    style WebApp fill:#e1f5fe
    style Gateway fill:#fff3e0
    style Services fill:#f3e5f5
    style Postgres fill:#e8f5e9
    style Core fill:#efebe9
```

## Layer Architecture

### Dependency Flow (Dependency Injection)

Each layer **only** depends on layers below it. No circular dependencies.

```mermaid
graph LR
    A[Blazor WebApp] -->|Uses HttpClient| B[Gadema.Api]
    B -->|Injects Services from| C[Services Layer]
    C -->|Depends on EF Core| D[Gadema.Data]
    D -->|Configurations for| E[Gadema.Core Models]
    
    style A fill:#e1f5fe
    style B fill:#fff3e0
    style C fill:#f3e5f5
    style D fill:#e8f5e9
    style E fill:#efebe9
```

## Data Flow Diagrams

### 1. Content Item Creation Flow (Full Lifecycle)

```mermaid
sequenceDiagram
    participant User as Writer
    participant Browser as Blazor Component
    participant API as Gadema.Api Controller
    participant Service as MetaInfoService
    participant DB as EF Core DbContext
    participant Audit as ActivityLog
    
    User->>Browser: Opens /content/create page
    Browser->>API: POST /api/content-items (CreateMetaInfoDto)
    
    rect rgb(240, 248, 255)
        Note over API, Service: Controller Layer
    end
    API->>Service: CreateAsync(dto, CurrentUser)
    
    rect rgb(255, 250, 231)
        Note over Service, Audit: Business Logic Layer
    end
    Service->>Service: Validate DTO (DataAnnotations)
    Service->>DB: Generate unique slug via hash(title + timestamp)
    Service->>DB: Create ContentVersionLog entry (audit trail)
    
    rect rgb(236, 253, 240)
        Note over DB: Data Access Layer
    end
    DB->>DB: INSERT into MetaInfos table
    DB-->>Service: Entity created
    
    Service-->>API: Response DTO with generated URL
    API-->>Browser: 201 Created response
    Browser-->>User: Success notification + refresh via SignalR
    
    Note over Audit: ActivityLog entry created:<br/>"MetaInfoCreated by User@X"
```

### 2. Branching Narrative Creation Flow (Self-Referencing Tree)

```mermaid
graph TD
    A[Create Root Node] -->|ParentNodeId = NULL| B[Root Node Created<br/>IsRoot = true]
    B --> C[Add Child Node A]
    C -->|ParentNodeId = Root.Id| D[Node A Created]
    D --> E[Add Nested Choice]
    E -->|ParentNodeId = NodeA.Id| F[Node B Created<br/>2-level deep tree]
    F --> G[Store Conditions JSON]
    
    subgraph Tree Structure
        H((Root))
        I((Node A))
        J((Node B))
        K((End Node))
        H --> I
        I --> J
        J -.->|optional condition| K
    end
    
    style H fill:#ffcccb stroke:#333 stroke-width:2px
    style I fill:#add8e6 stroke:#333 stroke-width:1px
    style J fill:#90ee90 stroke:#333 stroke-width:1px
```

### 3. ADHD-Friendly Task Filtering Flow

```mermaid
graph LR
    A[User clicks Easy Tasks] --> B{Query Filter}
    
    B -->|Status = InProgress| C[Filter by Status]
    C -->|Difficulty = Easy| D[Filter by Difficulty]
    D -->|IsQuickWin = true| E[Sort by estimatedMinutes ASC]
    E --> F[Return: Shortest tasks first<br/>with Quick Win badge]
    
    B -->|Status = Backlog| G[Filter by Status]
    G --> H{No difficulty filter}
    H --> I[Return all backlog items<br/>ordered by due date]
```

### 4. Authentication & Authorization Flow

```mermaid
flowchart TD
    A[Client sends request<br/>with Bearer token] --> B{Middleware Chain}
    
    B -->|1. CORS Check| C[CORS Middleware]
    C -->|2. JWT Validate| D[JWT Auth Middleware]
    D -->|3. Rate Limit| E[Rate Limiter]
    E -->|4. Policy Check| F[Policy: OwnProject<br/>AdminOnly, etc.]
    F -->|Authorized? NO| G[Return 401/403 Response]
    F -->|Authorized? YES| H{Endpoint Type}
    
    H -->|Auth Endpoint| I[Bypass Auth - Public]
    H -->|Protected| J[Execute Controller Action]
    
    subgraph JWT Token Decoded Claims
        K[userId]
        L[teamId optional]
        M[roles: Admin Lead Member]
    end
    
    style G fill:#ffebee stroke:#c62828
    style I fill:#e8f5e9 stroke:#2e7d32
```

## Database Schema Overview

### Entity Relationship Diagram

```mermaid
erDiagram
    User ||--o{ TeamMember : owns
    User }|--|| ProjectToken : has tokens
    Team ||--o{ TeamMember : contains
    Team ||--o{ Project : polymorphic owner
    
    Project ||--o{ MetaInfo : contains
    Project ||--o{ StorySequence : chapters
    Project ||--o{ ProjectTask : tasks
    Project ||--o{ ActivityLog : events
    
    MetaInfo ||--o{ DialogueBranch : has branches
    MetaInfo ||--o{ MediaAttachment : files
    MetaInfo }|--|| ReviewStatus : approval state
    
    StorySequence }o--|o{ StorySequence : parent chapter
    StorySequence ||--o{ StoryBeat : scenes
    StoryOutline }|--|| StorySequence : section of
    
    ProjectTask ||--o{ Comment : discussions on task
    ProjectTask |o--|| MetaInfo : optional link
    
    Tag }|--o{ ContentTagAssociation : 
    Tag }|--o{ ProjectTagAssociation : 
    
    style User fill:#e1f5fe stroke:#333 stroke-width:2px
    style Team fill:#fff3e0 stroke:#333 stroke-width:2px
    style Project fill:#f3e5f5 stroke:#333 stroke-width:2px
```

### Polymorphic Ownership Visual

```mermaid
graph TB
    subgraph User Projects ["User-Owned Projects"]
        U[User]
        P1[P1] -->|OwnerType 0 OwnerId=User.Id| U
        P2[P2] -->|OwnerType 0 OwnerId=User.Id| U
    end
    
    subgraph Team Projects ["Team-Owned Projects"]
        T[Team]
        P3[P3] -->|OwnerType 1 OwnerId=Team.Id| T
        P4[P4] -->|OwnerType 1 OwnerId=Team.Id| T
    end
    
    subgraph Projects Table ["Projects Table Single"]
        PK[Primary Key Id]
        FK[Foreign Key OwnerId]
        Disc[Int: OwnerType<br/>0=User 1=Team]
        
        P1 -->|same row as| PK
        P3 -->|same row as| PK
    end
    
    style Polymorphic Table fill:#e8f5e9 stroke-width:4px
```

## Security Architecture

### Authorization Policy Decision Matrix

| User Role | Create Project | Edit Own Content | Delete Projects | Export to Engine | View Public Content |
|-----------|---------------|-----------------|-----------------|-----------------|---------------------|
| User | Yes | Yes own only | No | Yes own | Yes |
| Team Member | Yes | Yes | No | Yes | Yes team projects |
| Team Lead | Yes | Yes assign tasks | No | Yes | Yes |
| Admin | Yes | Yes all | Yes transfer/delete | Yes any | All |

## Deployment Architecture

```mermaid
graph TB
    subgraph Production ["Production"]
        LB[Load Balancer<br/>NGINX / Cloudflare]
        
        subgraph App Tier ["App Tier Blazor Server"]
            WS1[Worker 1<br/>SignalR Hub Hosted]
            WS2[Worker 2<br/>SignalR Hub Hosted]
            WS3[Worker 3<br/>SignalR Hub Hosted]
            
            LB --> WS1
            LB --> WS2
            LB --> WS3
        end
        
        subgraph API Tier ["API Tier ASP.NET Core"]
            AP1[API Server 1]
            AP2[API Server 2]
            AP3[API Server 3]
            
            LB --> AP1
            LB --> AP2
            LB --> AP3
            
            AP1 -->|Postgres ReadWrite| PG[(Primary DB)]
            AP2 -->|Read Replica| PGR[(Replica DB)]
        end
        
        subgraph Infrastructure ["Infrastructure"]
            Redis[(Redis Cache<br/>hot data)]
        end
        
        WS1 -.-> Redis
        WS2 -.-> Redis
    end
    
    style LB fill:#fff3e0 stroke-width:4px
    style PG fill:#e8f5e9
```

**Scaling Considerations:**

- Blazor Server is stateful — sticky sessions via SignalR handle distribution
- API layer scales horizontally — no shared in-memory state
- Redis caches session tokens and optimizes real-time broadcast
- Database read replicas handle heavy content queries

## Component Communication Patterns

### SignalR Real-Time Updates

```mermaid
sequenceDiagram
    participant Writer as Writer blazor
    participant Hub as SignalR Hub
    participant API as ContentService
    
    Writer->>Hub: Subscribe to project123-changes group
    Note over Writer,Hub: Client maintains connection<br/>via WebSocket + HTTP fallback
    
    Actor API->>API: User edits content
    API->>Hub: Broadcast content-updated id title
    
    rect rgb(240, 255, 240)
        Note over Hub, Writer: All subscribed clients<br/>receive update instantly
    end
    
    Hub-->>Writer1: MetaInfoChanged event
    Hub-->>Writer2: MetaInfoChanged event
    Hub-->>EditorC: MetaInfoChanged event
    
    Writer1->>Writer1: UI refreshes via StateHasChanged()
    Writer2->>Writer2: UI refreshes via StateHasChanged()
```

### Service-to-Service Communication (Mediator Pattern)

```mermaid
graph LR
    A[ContentService] -->|IEventPublisher.Publish| B{Message Bus}
    
    B --> C[VersionLogCreatedEvent]
    B --> D[ActivityLoggedEvent]
    B --> E[ReviewAssignedEvent]
    
    C -.-> F[ContentVersionService]
    D -.-> G[ActivityLogService]
    E -.-> H[NotificationService]
```

## Performance Optimization Strategies

### Query Pattern Library

| Pattern | Use Case | Example |
|---------|----------|---------|
| **Eager Loading** | Fetch nested data in one query | `.Include(ci => ci.MediaAttachments)` |
| **Projection Select** | Reduce payload size | `.Select(p => new ProjectSummaryDto(...))` |
| **Pagination** | Large lists | `Skip().Take(20)` with index |
| **Filtering Indexes** | Common filters | IX_MetaInfos_Status IX_ProjectTasks_Difficulty |
| **Soft Delete Filter** | Preserve history | `.Where(p => p.IsActive == true)` |

### Caching Strategy (Redis)

```mermaid
graph TD
    A[API Request] --> B{Cache Check}
    
    B -->|Hit 1 hr TTL| C[(Cache Published Content)]
    C --> D[Return cached response]
    
    B -->|Miss| E[Query Database]
    E --> F[(PostgreSQL)]
    F --> G[Set in Redis for<br/>published: 1 hour<br/>drafts: 5 min<br/>tasks: 2 min]
    G --> H[Return to client]
```

**TTL Rules:**
- Published content: 1 hour (rarely changes)
- Draft content: 5 minutes (frequently edited)
- Task state: 2 minutes (high-frequency updates)

## Error Handling Strategy

### Exception Flow Through the Stack

```mermaid
graph TD
    A[Controller Action] --> B{Exception Caught?}
    
    B -->|Yes| C{Exception Type}
    B -->|No| D[Execute Logic]
    
    C -->|NotFoundException| E[Return 404 JSON]
    C -->|ValidationException| F[Return 400 with error list]
    C -->|UnauthorizedException| G[Return 401]
    C -->|ForbiddenAccessException| H[Return 403]
    C -->|DbUpdateException| I[Return 500 database error message]
    C -->|OperationCanceledException| J[Timeout response]
    
    D --> K{Business Exception?}
    K -->|Yes| C
    K -->|No| L[Return 200 OK]
    
    style E fill:#fff3e0 stroke:#f57c00
    style F fill:#ffebee stroke:#c62828
    style G fill:#fce4ec stroke:#c2185b
    style H fill:#e8f5e9 stroke:#388e3c
```

### Controller Layer Responsibility

Controllers are **thin** — they only:

1. Receive DTOs from clients
2. Validate with `[Required]` attributes or FluentValidation
3. Call service layer methods
4. Return response DTOs

They **never**:
- Directly access the database (`DbContext`)
- Perform business logic (that's in Services)
- Handle side effects like email sending (in Services/Middleware)

This separation means controllers can be replaced with gRPC, GraphQL, or a different HTTP framework without touching business logic.