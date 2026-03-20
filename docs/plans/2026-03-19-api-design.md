# Mnemosyne API Design

**Date:** 2026-03-19  
**Status:** Approved  
**Author:** Product Team

---

## Context

Mnemosyne is a Memory & Context Intelligence Platform for AI agents. This document captures the approved API design decisions made during the brainstorming session, based on PRD v1.0.

---

## Architecture

### Solution Structure

```
Mnemosyne.sln
├── src/
│   ├── Mnemosyne.Api/              # ASP.NET Core 10, REST + gRPC endpoints
│   ├── Mnemosyne.Core/             # Domain models, DTOs, Interfaces, Feature folders
│   └── Mnemosyne.Infrastructure/   # EF Core, Dapper, pgvector, Redis, OpenAI
└── tests/
    ├── Mnemosyne.Api.Tests/
    ├── Mnemosyne.Core.Tests/
    └── Mnemosyne.Infrastructure.Tests/
```

### Project Organization (Feature Folders)

`Mnemosyne.Core` uses feature folders:

```
Mnemosyne.Core/
├── Memory/
│   ├── IMemoryRepository.cs
│   ├── MemoryEntity.cs
│   ├── MemoryDto.cs
│   └── MemoryService.cs
├── Project/
│   ├── IProjectRepository.cs
│   ├── ProjectEntity.cs
│   ├── ProjectDto.cs
│   └── ProjectService.cs
├── Search/
│   ├── ISearchService.cs
│   ├── SearchResult.cs
│   └── SearchService.cs
├── Index/
│   ├── IIndexService.cs
│   ├── IndexStatus.cs
│   └── IndexService.cs
├── Compress/
│   ├── ICompressService.cs
│   ├── CompressResult.cs
│   └── CompressService.cs
└── Auth/
    ├── IAuthService.cs
    ├── ApiKeyEntity.cs
    └── AuthService.cs
```

---

## Technology Stack

| Component | Technology | Notes |
|-----------|------------|-------|
| REST API | ASP.NET Core 10 Minimal APIs | Primary interface for CRUD |
| gRPC API | Grpc.AspNetCore + Protobuf | High-performance: index, search, compress |
| ORM (CRUD) | EF Core 10 | Migrations, simple queries |
| ORM (Complex) | Dapper | Vector search queries, analytics |
| Database | PostgreSQL 17.9 + pgvector 0.8.2 | Memories, embeddings, FTS |
| Vector Support | Npgsql.Vector | pgvector integration for .NET |
| Cache | Redis 7.x | L1 cache, rate limiting |
| Authentication | API Key + bcrypt | Authorization: Bearer header |
| Embeddings | OpenAI text-embedding-3-small | External API, abstraction layer planned |
| Logging | Microsoft.Extensions.Logging | Built-in, structured |
| Health Checks | ASP.NET Health Checks | DB, Redis, OpenAI connectivity |
| CLI | Bun + TypeScript | Separate tool, calls REST API |

---

## API Design

### REST Endpoints (Minimal APIs)

#### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/validate` | Validate API Key, return user info |
| POST | `/api/v1/auth/regenerate` | Regenerate API Key |
| GET | `/api/v1/auth/usage` | Current usage and plan limits |

#### Memory
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/memory` | Create memory with auto-embedding |
| GET | `/api/v1/memory/:id` | Get memory by ID |
| GET | `/api/v1/memory/search` | Semantic search (query param) |
| PUT | `/api/v1/memory/:id` | Update memory content |
| DELETE | `/api/v1/memory/:id` | Delete memory |
| GET | `/api/v1/memory` | List memories with filters |

#### Project
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/project` | Create project |
| GET | `/api/v1/project/:id` | Project details + stats |
| GET | `/api/v1/project` | List user projects |
| PUT | `/api/v1/project/:id` | Update project settings |
| DELETE | `/api/v1/project/:id` | Delete project + associated data |
| POST | `/api/v1/project/:id/index` | Trigger async indexing |

### gRPC Services

```protobuf
service IndexService {
    rpc IndexProject(IndexRequest) returns (stream IndexProgress);
    rpc GetIndexStatus(StatusRequest) returns (IndexStatus);
}

service SearchService {
    rpc SemanticSearch(SearchRequest) returns (SearchResponse);
    rpc KeywordSearch(SearchRequest) returns (SearchResponse);
    rpc HybridSearch(SearchRequest) returns (SearchResponse);
}

service CompressService {
    rpc CompressCode(CompressRequest) returns (stream CompressChunk);
    rpc CompressContext(CompressRequest) returns (stream CompressChunk);
}
```

### Health Checks

```
GET /health/live    # Liveness: app is running
GET /health/ready   # Readiness: DB, Redis, OpenAI reachable
```

---

## Data Model

```csharp
// EF Core entities (simplified)

Users         { Id, Email, ApiKeyHash, Tier, CreatedAt }
Projects      { Id, UserId, Name, Path, Settings, CreatedAt }
Memories      { Id, ProjectId, Type, Content, Embedding(vector), Tags, CreatedAt }
CodeChunks    { Id, ProjectId, FilePath, Content, Embedding(vector), Language, CreatedAt }
UsageLogs     { Id, UserId, Endpoint, TokensUsed, LatencyMs, CreatedAt }
```

Vector columns use `Npgsql.Vector` type mapped to `pgvector` extension.

---

## Authentication Flow

1. User receives `api_key` on registration
2. Key stored as `bcrypt(api_key)` in `users.api_key_hash`
3. Every request: `Authorization: Bearer <api_key>` header
4. Middleware validates: hash comparison, rate limit check
5. Failure: `401 Unauthorized`

---

## Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| API Framework | ASP.NET Core 10 | Performance, mature ecosystem, gRPC first-class |
| Architecture | Modular Monolith | MVP speed, single deployment, easier to split later |
| Data access | EF Core + Dapper hybrid | EF for CRUD/migrations, Dapper for vector queries |
| Auth | API Key + bcrypt | Simple, secure, sufficient for MVP |
| Embeddings | OpenAI | Best quality, simple integration, abstraction planned |
| CLI | Bun + TypeScript | Lighter than .NET console, aligned with SKILL ecosystem |
| Structure | Feature folders | Better cohesion than layer folders for domain features |

---

## Out of Scope (MVP)

- OAuth2 / JWT authentication
- Multi-provider embeddings (local models)
- Kubernetes / container orchestration configs
- Serilog / OpenTelemetry (add in Phase 4)
- Dashboard (separate Next.js app, Phase 3)
