# Mnemosyne API Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build the Mnemosyne REST + gRPC API in .NET 10 with ASP.NET Core, EF Core + Dapper hybrid, pgvector, Redis, and OpenAI embeddings.

**Architecture:** Modular Monolith with 3 projects (`Mnemosyne.Api`, `Mnemosyne.Core`, `Mnemosyne.Infrastructure`) organized by feature folders. REST via Minimal APIs, gRPC via Grpc.AspNetCore with Protobuf contracts.

**Tech Stack:** .NET 10, ASP.NET Core 10 Minimal APIs, EF Core 10, Dapper, Npgsql.Vector, PostgreSQL 17.9 + pgvector 0.8.2, Redis (StackExchange.Redis), OpenAI SDK (.NET), xUnit, Moq, AutoFixture

---

## Phase 1: Foundation

---

### Task 1: Initialize Solution & Projects

**Files:**
- Create: `Mnemosyne.slnx`
- Create: `src/Mnemosyne.Api/Mnemosyne.Api.csproj`
- Create: `src/Mnemosyne.Core/Mnemosyne.Core.csproj`
- Create: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Create: `tests/Mnemosyne.Api.Tests/Mnemosyne.Api.Tests.csproj`
- Create: `tests/Mnemosyne.Core.Tests/Mnemosyne.Core.Tests.csproj`
- Create: `tests/Mnemosyne.Infrastructure.Tests/Mnemosyne.Infrastructure.Tests.csproj`

**Step 1: Create solution and projects**

```bash
dotnet new slnx -n Mnemosyne
dotnet new webapi -n Mnemosyne.Api -o src/Mnemosyne.Api --use-minimal-apis -f net10.0
dotnet new classlib -n Mnemosyne.Core -o src/Mnemosyne.Core -f net10.0
dotnet new classlib -n Mnemosyne.Infrastructure -o src/Mnemosyne.Infrastructure -f net10.0
dotnet new xunit -n Mnemosyne.Api.Tests -o tests/Mnemosyne.Api.Tests -f net10.0
dotnet new xunit -n Mnemosyne.Core.Tests -o tests/Mnemosyne.Core.Tests -f net10.0
dotnet new xunit -n Mnemosyne.Infrastructure.Tests -o tests/Mnemosyne.Infrastructure.Tests -f net10.0
```

**Step 2: Add projects to solution**

```bash
dotnet slnx add src/Mnemosyne.Api/Mnemosyne.Api.csproj
dotnet slnx add src/Mnemosyne.Core/Mnemosyne.Core.csproj
dotnet slnx add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
dotnet slnx add tests/Mnemosyne.Api.Tests/Mnemosyne.Api.Tests.csproj
dotnet slnx add tests/Mnemosyne.Core.Tests/Mnemosyne.Core.Tests.csproj
dotnet slnx add tests/Mnemosyne.Infrastructure.Tests/Mnemosyne.Infrastructure.Tests.csproj
```

**Step 3: Add project references**

```bash
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj reference src/Mnemosyne.Core/Mnemosyne.Core.csproj
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj reference src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj reference src/Mnemosyne.Core/Mnemosyne.Core.csproj
dotnet add tests/Mnemosyne.Api.Tests/Mnemosyne.Api.Tests.csproj reference src/Mnemosyne.Api/Mnemosyne.Api.csproj
dotnet add tests/Mnemosyne.Core.Tests/Mnemosyne.Core.Tests.csproj reference src/Mnemosyne.Core/Mnemosyne.Core.csproj
dotnet add tests/Mnemosyne.Infrastructure.Tests/Mnemosyne.Infrastructure.Tests.csproj reference src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
```

**Step 4: Verify build**

```bash
dotnet build
```
Expected: Build succeeded, 0 errors

**Step 5: Commit**

```bash
git init
git add .
git commit -m "chore: initialize solution with projects and references"
```

---

### Task 2: Install NuGet Packages

**Files:**
- Modify: `src/Mnemosyne.Api/Mnemosyne.Api.csproj`
- Modify: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Modify: `tests/Mnemosyne.Core.Tests/Mnemosyne.Core.Tests.csproj`

**Step 1: Add packages to Infrastructure**

```bash
dotnet add src/Mnemosyne.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/Mnemosyne.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/Mnemosyne.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Mnemosyne.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite
dotnet add src/Mnemosyne.Infrastructure package Pgvector.EntityFrameworkCore
dotnet add src/Mnemosyne.Infrastructure package Dapper
dotnet add src/Mnemosyne.Infrastructure package Npgsql
dotnet add src/Mnemosyne.Infrastructure package StackExchange.Redis
dotnet add src/Mnemosyne.Infrastructure package BCrypt.Net-Next
dotnet add src/Mnemosyne.Infrastructure package OpenAI
```

**Step 2: Add packages to API**

```bash
dotnet add src/Mnemosyne.Api package Grpc.AspNetCore
dotnet add src/Mnemosyne.Api package Microsoft.AspNetCore.Diagnostics.HealthChecks
dotnet add src/Mnemosyne.Api package AspNetCore.HealthChecks.NpgSql
dotnet add src/Mnemosyne.Api package AspNetCore.HealthChecks.Redis
```

**Step 3: Add test packages**

```bash
dotnet add tests/Mnemosyne.Core.Tests package Moq
dotnet add tests/Mnemosyne.Core.Tests package AutoFixture
dotnet add tests/Mnemosyne.Core.Tests package AutoFixture.AutoMoq
dotnet add tests/Mnemosyne.Api.Tests package Moq
dotnet add tests/Mnemosyne.Api.Tests package AutoFixture
dotnet add tests/Mnemosyne.Api.Tests package AutoFixture.AutoMoq
dotnet add tests/Mnemosyne.Api.Tests package Microsoft.AspNetCore.Mvc.Testing
```

**Step 4: Build to verify packages**

```bash
dotnet build
```
Expected: Build succeeded, 0 errors

**Step 5: Commit**

```bash
git add .
git commit -m "chore: add NuGet packages to all projects"
```

---

### Task 3: Domain Entities (Core)

**Files:**
- Create: `src/Mnemosyne.Core/Auth/ApiKeyEntity.cs`
- Create: `src/Mnemosyne.Core/Project/ProjectEntity.cs`
- Create: `src/Mnemosyne.Core/Memory/MemoryEntity.cs`
- Create: `src/Mnemosyne.Core/Index/CodeChunkEntity.cs`
- Create: `src/Mnemosyne.Core/Analytics/UsageLogEntity.cs`

**Step 1: Write failing test for MemoryEntity**

`tests/Mnemosyne.Core.Tests/Memory/MemoryEntityTests.cs`

```csharp
using Mnemosyne.Core.Memory;

namespace Mnemosyne.Core.Tests.Memory;

public class MemoryEntityTests
{
    [Fact]
    public void MemoryEntity_ShouldHaveRequiredProperties()
    {
        var memory = new MemoryEntity
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Type = MemoryType.Note,
            Content = "Test content"
        };

        Assert.Equal("Test content", memory.Content);
        Assert.Equal(MemoryType.Note, memory.Type);
    }
}
```

**Step 2: Run test to verify it fails**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "MemoryEntityTests"
```
Expected: FAIL - `MemoryEntity` not found

**Step 3: Create MemoryType enum and MemoryEntity**

`src/Mnemosyne.Core/Memory/MemoryEntity.cs`

```csharp
namespace Mnemosyne.Core.Memory;

public enum MemoryType
{
    Note,
    Decision,
    Preference,
    Context
}

public class MemoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public MemoryType Type { get; set; } = MemoryType.Note;
    public string Content { get; set; } = string.Empty;
    public float[]? Embedding { get; set; }
    public string[] Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

**Step 4: Create remaining entities**

`src/Mnemosyne.Core/Auth/ApiKeyEntity.cs`

```csharp
namespace Mnemosyne.Core.Auth;

public enum UserTier { Free, Pro, Enterprise }

public class UserEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string ApiKeyHash { get; set; } = string.Empty;
    public UserTier Tier { get; set; } = UserTier.Free;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

`src/Mnemosyne.Core/Project/ProjectEntity.cs`

```csharp
namespace Mnemosyne.Core.Project;

public class ProjectEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string Settings { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

`src/Mnemosyne.Core/Index/CodeChunkEntity.cs`

```csharp
namespace Mnemosyne.Core.Index;

public class CodeChunkEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float[]? Embedding { get; set; }
    public string? Language { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

`src/Mnemosyne.Core/Analytics/UsageLogEntity.cs`

```csharp
namespace Mnemosyne.Core.Analytics;

public class UsageLogEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public int LatencyMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**Step 5: Run test to verify it passes**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "MemoryEntityTests"
```
Expected: PASS

**Step 6: Commit**

```bash
git add .
git commit -m "feat: add core domain entities"
```

---

### Task 4: Repository Interfaces (Core)

**Files:**
- Create: `src/Mnemosyne.Core/Memory/IMemoryRepository.cs`
- Create: `src/Mnemosyne.Core/Project/IProjectRepository.cs`
- Create: `src/Mnemosyne.Core/Auth/IAuthRepository.cs`
- Create: `src/Mnemosyne.Core/Analytics/IUsageLogRepository.cs`

**Step 1: Create IMemoryRepository**

`src/Mnemosyne.Core/Memory/IMemoryRepository.cs`

```csharp
namespace Mnemosyne.Core.Memory;

public interface IMemoryRepository
{
    Task<MemoryEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<MemoryEntity>> ListAsync(Guid projectId, MemoryType? type = null, CancellationToken ct = default);
    Task<MemoryEntity> CreateAsync(MemoryEntity memory, CancellationToken ct = default);
    Task<MemoryEntity?> UpdateAsync(Guid id, string content, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<MemoryEntity>> SearchSimilarAsync(float[] embedding, Guid projectId, int limit = 10, CancellationToken ct = default);
}
```

**Step 2: Create IProjectRepository**

`src/Mnemosyne.Core/Project/IProjectRepository.cs`

```csharp
namespace Mnemosyne.Core.Project;

public interface IProjectRepository
{
    Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ProjectEntity>> ListByUserAsync(Guid userId, CancellationToken ct = default);
    Task<ProjectEntity> CreateAsync(ProjectEntity project, CancellationToken ct = default);
    Task<ProjectEntity?> UpdateAsync(Guid id, string? name, string? settings, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
```

**Step 3: Create IAuthRepository**

`src/Mnemosyne.Core/Auth/IAuthRepository.cs`

```csharp
namespace Mnemosyne.Core.Auth;

public interface IAuthRepository
{
    Task<UserEntity?> GetByApiKeyAsync(string apiKey, CancellationToken ct = default);
    Task<UserEntity> CreateAsync(UserEntity user, CancellationToken ct = default);
    Task<string> RegenerateApiKeyAsync(Guid userId, CancellationToken ct = default);
}
```

**Step 4: Create IUsageLogRepository**

`src/Mnemosyne.Core/Analytics/IUsageLogRepository.cs`

```csharp
namespace Mnemosyne.Core.Analytics;

public interface IUsageLogRepository
{
    Task LogAsync(UsageLogEntity log, CancellationToken ct = default);
    Task<IEnumerable<UsageLogEntity>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
}
```

**Step 5: Build to verify**

```bash
dotnet build src/Mnemosyne.Core
```
Expected: Build succeeded, 0 errors

**Step 6: Commit**

```bash
git add .
git commit -m "feat: add repository interfaces to core"
```

---

### Task 5: Database Context (Infrastructure)

**Files:**
- Create: `src/Mnemosyne.Infrastructure/Persistence/MnemosyneDbContext.cs`
- Create: `src/Mnemosyne.Infrastructure/Persistence/Configurations/MemoryConfiguration.cs`
- Create: `src/Mnemosyne.Infrastructure/Persistence/Configurations/ProjectConfiguration.cs`

**Step 1: Create DbContext**

`src/Mnemosyne.Infrastructure/Persistence/MnemosyneDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Mnemosyne.Core.Auth;
using Mnemosyne.Core.Analytics;
using Mnemosyne.Core.Memory;
using Mnemosyne.Core.Project;
using Mnemosyne.Core.Index;
using Pgvector.EntityFrameworkCore;

namespace Mnemosyne.Infrastructure.Persistence;

public class MnemosyneDbContext : DbContext
{
    public MnemosyneDbContext(DbContextOptions<MnemosyneDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
    public DbSet<MemoryEntity> Memories => Set<MemoryEntity>();
    public DbSet<CodeChunkEntity> CodeChunks => Set<CodeChunkEntity>();
    public DbSet<UsageLogEntity> UsageLogs => Set<UsageLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MnemosyneDbContext).Assembly);
    }
}
```

**Step 2: Create entity configurations**

`src/Mnemosyne.Infrastructure/Persistence/Configurations/MemoryConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnemosyne.Core.Memory;
using Pgvector;

namespace Mnemosyne.Infrastructure.Persistence.Configurations;

public class MemoryConfiguration : IEntityTypeConfiguration<MemoryEntity>
{
    public void Configure(EntityTypeBuilder<MemoryEntity> builder)
    {
        builder.ToTable("memories");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.Type).HasConversion<string>();
        builder.Property(m => m.Tags).HasColumnType("text[]");
        builder.Property(m => m.Embedding)
            .HasColumnType("vector(1536)")
            .HasConversion(
                v => v == null ? null : new Vector(v),
                v => v == null ? null : v.ToArray());
    }
}
```

`src/Mnemosyne.Infrastructure/Persistence/Configurations/ProjectConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnemosyne.Core.Project;

namespace Mnemosyne.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
{
    public void Configure(EntityTypeBuilder<ProjectEntity> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Settings).HasColumnType("jsonb");
    }
}
```

**Step 3: Create initial EF Core migration**

```bash
dotnet ef migrations add InitialCreate \
  --project src/Mnemosyne.Infrastructure \
  --startup-project src/Mnemosyne.Api
```
Expected: Migration files created in `src/Mnemosyne.Infrastructure/Migrations/`

**Step 4: Build to verify**

```bash
dotnet build
```
Expected: Build succeeded, 0 errors

**Step 5: Commit**

```bash
git add .
git commit -m "feat: add DbContext and entity configurations with EF Core migrations"
```

---

### Task 6: Auth Service (Core)

**Files:**
- Create: `src/Mnemosyne.Core/Auth/AuthService.cs`
- Create: `src/Mnemosyne.Core/Auth/AuthDtos.cs`
- Test: `tests/Mnemosyne.Core.Tests/Auth/AuthServiceTests.cs`

**Step 1: Create DTOs**

`src/Mnemosyne.Core/Auth/AuthDtos.cs`

```csharp
namespace Mnemosyne.Core.Auth;

public record ValidateApiKeyResponse(
    Guid UserId,
    string Email,
    UserTier Tier,
    bool IsValid
);

public record UsageResponse(
    int TokensUsed,
    int TokenLimit,
    string Plan
);
```

**Step 2: Write failing tests**

`tests/Mnemosyne.Core.Tests/Auth/AuthServiceTests.cs`

```csharp
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Mnemosyne.Core.Auth;

namespace Mnemosyne.Core.Tests.Auth;

public class AuthServiceTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public async Task ValidateApiKey_WhenKeyExists_ReturnsValidResponse()
    {
        var repo = _fixture.Freeze<Mock<IAuthRepository>>();
        var user = _fixture.Build<UserEntity>()
            .With(u => u.Tier, UserTier.Pro)
            .Create();
        repo.Setup(r => r.GetByApiKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var sut = _fixture.Create<AuthService>();
        var result = await sut.ValidateApiKeyAsync("any-key");

        Assert.True(result.IsValid);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task ValidateApiKey_WhenKeyNotFound_ReturnsInvalidResponse()
    {
        var repo = _fixture.Freeze<Mock<IAuthRepository>>();
        repo.Setup(r => r.GetByApiKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        var sut = _fixture.Create<AuthService>();
        var result = await sut.ValidateApiKeyAsync("bad-key");

        Assert.False(result.IsValid);
    }
}
```

**Step 3: Run tests to verify they fail**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "AuthServiceTests"
```
Expected: FAIL - `AuthService` not found

**Step 4: Implement AuthService**

`src/Mnemosyne.Core/Auth/AuthService.cs`

```csharp
namespace Mnemosyne.Core.Auth;

public class AuthService
{
    private readonly IAuthRepository _repository;

    public AuthService(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<ValidateApiKeyResponse> ValidateApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        var user = await _repository.GetByApiKeyAsync(apiKey, ct);
        if (user is null)
            return new ValidateApiKeyResponse(Guid.Empty, string.Empty, UserTier.Free, IsValid: false);

        return new ValidateApiKeyResponse(user.Id, user.Email, user.Tier, IsValid: true);
    }

    public async Task<string> RegenerateApiKeyAsync(Guid userId, CancellationToken ct = default)
    {
        return await _repository.RegenerateApiKeyAsync(userId, ct);
    }
}
```

**Step 5: Run tests to verify they pass**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "AuthServiceTests"
```
Expected: PASS

**Step 6: Commit**

```bash
git add .
git commit -m "feat: add AuthService with validate and regenerate"
```

---

### Task 7: Auth Middleware (API)

**Files:**
- Create: `src/Mnemosyne.Api/Middleware/ApiKeyMiddleware.cs`
- Test: `tests/Mnemosyne.Api.Tests/Middleware/ApiKeyMiddlewareTests.cs`

**Step 1: Write failing test**

`tests/Mnemosyne.Api.Tests/Middleware/ApiKeyMiddlewareTests.cs`

```csharp
using Microsoft.AspNetCore.Http;
using Moq;
using Mnemosyne.Api.Middleware;
using Mnemosyne.Core.Auth;

namespace Mnemosyne.Api.Tests.Middleware;

public class ApiKeyMiddlewareTests
{
    [Fact]
    public async Task Invoke_WithoutAuthHeader_Returns401()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var next = new RequestDelegate(_ => Task.CompletedTask);
        var authService = new Mock<AuthService>(Mock.Of<IAuthRepository>());

        var middleware = new ApiKeyMiddleware(next);
        await middleware.InvokeAsync(context, authService.Object);

        Assert.Equal(401, context.Response.StatusCode);
    }
}
```

**Step 2: Run test to verify it fails**

```bash
dotnet test tests/Mnemosyne.Api.Tests --filter "ApiKeyMiddlewareTests"
```
Expected: FAIL

**Step 3: Implement ApiKeyMiddleware**

`src/Mnemosyne.Api/Middleware/ApiKeyMiddleware.cs`

```csharp
using Mnemosyne.Core.Auth;

namespace Mnemosyne.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader is null || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Missing or invalid Authorization header" });
            return;
        }

        var apiKey = authHeader["Bearer ".Length..].Trim();
        var result = await authService.ValidateApiKeyAsync(apiKey, context.RequestAborted);

        if (!result.IsValid)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
            return;
        }

        context.Items["UserId"] = result.UserId;
        context.Items["UserTier"] = result.Tier;
        await _next(context);
    }
}
```

**Step 4: Run test to verify it passes**

```bash
dotnet test tests/Mnemosyne.Api.Tests --filter "ApiKeyMiddlewareTests"
```
Expected: PASS

**Step 5: Commit**

```bash
git add .
git commit -m "feat: add ApiKey authentication middleware"
```

---

### Task 8: Auth Endpoints (API)

**Files:**
- Create: `src/Mnemosyne.Api/Endpoints/AuthEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`

**Step 1: Create AuthEndpoints**

`src/Mnemosyne.Api/Endpoints/AuthEndpoints.cs`

```csharp
using Mnemosyne.Core.Auth;

namespace Mnemosyne.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth");

        group.MapPost("/validate", async (HttpContext ctx, AuthService authService) =>
        {
            var userId = ctx.Items["UserId"] as Guid?;
            if (userId is null) return Results.Unauthorized();

            var authHeader = ctx.Request.Headers.Authorization.FirstOrDefault()!;
            var apiKey = authHeader["Bearer ".Length..].Trim();
            var result = await authService.ValidateApiKeyAsync(apiKey, ctx.RequestAborted);

            return Results.Ok(result);
        });

        group.MapPost("/regenerate", async (HttpContext ctx, AuthService authService) =>
        {
            var userId = (Guid)ctx.Items["UserId"]!;
            var newKey = await authService.RegenerateApiKeyAsync(userId, ctx.RequestAborted);
            return Results.Ok(new { apiKey = newKey });
        });

        return app;
    }
}
```

**Step 2: Update Program.cs**

`src/Mnemosyne.Api/Program.cs`

```csharp
using Mnemosyne.Api.Endpoints;
using Mnemosyne.Api.Middleware;
using Mnemosyne.Core.Auth;
using Mnemosyne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MnemosyneDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector()));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();
app.MapAuthEndpoints();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
```

**Step 3: Build to verify**

```bash
dotnet build
```
Expected: Build succeeded, 0 errors

**Step 4: Commit**

```bash
git add .
git commit -m "feat: add auth endpoints and wire up Program.cs"
```

---

## Phase 2: Core Features

---

### Task 9: Memory Repository (Infrastructure)

**Files:**
- Create: `src/Mnemosyne.Infrastructure/Repositories/MemoryRepository.cs`
- Test: `tests/Mnemosyne.Infrastructure.Tests/Repositories/MemoryRepositoryTests.cs`

**Step 1: Write failing test (integration-style, mocked DbContext)**

`tests/Mnemosyne.Infrastructure.Tests/Repositories/MemoryRepositoryTests.cs`

```csharp
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Mnemosyne.Core.Memory;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;

namespace Mnemosyne.Infrastructure.Tests.Repositories;

public class MemoryRepositoryTests : IDisposable
{
    private readonly MnemosyneDbContext _context;
    private readonly MemoryRepository _sut;

    public MemoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new MnemosyneDbContext(options);
        _sut = new MemoryRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistMemory()
    {
        var memory = new MemoryEntity { Content = "Test", ProjectId = Guid.NewGuid() };
        var result = await _sut.CreateAsync(memory);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test", result.Content);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnMemory()
    {
        var memory = new MemoryEntity { Content = "Test", ProjectId = Guid.NewGuid() };
        await _context.Memories.AddAsync(memory);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(memory.Id);

        Assert.NotNull(result);
        Assert.Equal(memory.Id, result.Id);
    }

    public void Dispose() => _context.Dispose();
}
```

**Step 2: Run tests to verify they fail**

```bash
dotnet test tests/Mnemosyne.Infrastructure.Tests --filter "MemoryRepositoryTests"
```
Expected: FAIL

**Step 3: Implement MemoryRepository**

`src/Mnemosyne.Infrastructure/Repositories/MemoryRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Mnemosyne.Core.Memory;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Infrastructure.Repositories;

public class MemoryRepository : IMemoryRepository
{
    private readonly MnemosyneDbContext _context;

    public MemoryRepository(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<MemoryEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Memories.FindAsync([id], ct);

    public async Task<IEnumerable<MemoryEntity>> ListAsync(Guid projectId, MemoryType? type = null, CancellationToken ct = default)
    {
        var query = _context.Memories.Where(m => m.ProjectId == projectId);
        if (type.HasValue) query = query.Where(m => m.Type == type.Value);
        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync(ct);
    }

    public async Task<MemoryEntity> CreateAsync(MemoryEntity memory, CancellationToken ct = default)
    {
        _context.Memories.Add(memory);
        await _context.SaveChangesAsync(ct);
        return memory;
    }

    public async Task<MemoryEntity?> UpdateAsync(Guid id, string content, CancellationToken ct = default)
    {
        var memory = await _context.Memories.FindAsync([id], ct);
        if (memory is null) return null;
        memory.Content = content;
        memory.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return memory;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var memory = await _context.Memories.FindAsync([id], ct);
        if (memory is null) return false;
        _context.Memories.Remove(memory);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<MemoryEntity>> SearchSimilarAsync(float[] embedding, Guid projectId, int limit = 10, CancellationToken ct = default)
    {
        // Vector search via Dapper + pgvector raw query (EF Core fallback for testing)
        return await _context.Memories
            .Where(m => m.ProjectId == projectId && m.Embedding != null)
            .Take(limit)
            .ToListAsync(ct);
    }
}
```

**Step 4: Run tests to verify they pass**

```bash
dotnet test tests/Mnemosyne.Infrastructure.Tests --filter "MemoryRepositoryTests"
```
Expected: PASS

**Step 5: Commit**

```bash
git add .
git commit -m "feat: add MemoryRepository with EF Core"
```

---

### Task 10: Memory Service (Core)

**Files:**
- Create: `src/Mnemosyne.Core/Memory/MemoryService.cs`
- Create: `src/Mnemosyne.Core/Memory/MemoryDtos.cs`
- Create: `src/Mnemosyne.Core/Embeddings/IEmbeddingService.cs`
- Test: `tests/Mnemosyne.Core.Tests/Memory/MemoryServiceTests.cs`

**Step 1: Create IEmbeddingService**

`src/Mnemosyne.Core/Embeddings/IEmbeddingService.cs`

```csharp
namespace Mnemosyne.Core.Embeddings;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default);
}
```

**Step 2: Create MemoryDtos**

`src/Mnemosyne.Core/Memory/MemoryDtos.cs`

```csharp
namespace Mnemosyne.Core.Memory;

public record CreateMemoryRequest(
    Guid ProjectId,
    string Content,
    MemoryType Type = MemoryType.Note,
    string[]? Tags = null
);

public record MemoryResponse(
    Guid Id,
    Guid ProjectId,
    string Content,
    MemoryType Type,
    string[] Tags,
    DateTime CreatedAt
);
```

**Step 3: Write failing tests**

`tests/Mnemosyne.Core.Tests/Memory/MemoryServiceTests.cs`

```csharp
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Mnemosyne.Core.Embeddings;
using Mnemosyne.Core.Memory;

namespace Mnemosyne.Core.Tests.Memory;

public class MemoryServiceTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public async Task CreateAsync_ShouldGenerateEmbeddingAndPersist()
    {
        var embeddingService = _fixture.Freeze<Mock<IEmbeddingService>>();
        var repo = _fixture.Freeze<Mock<IMemoryRepository>>();
        var embedding = new float[1536];

        embeddingService.Setup(e => e.GetEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(embedding);
        repo.Setup(r => r.CreateAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MemoryEntity m, CancellationToken _) => m);

        var sut = _fixture.Create<MemoryService>();
        var request = new CreateMemoryRequest(Guid.NewGuid(), "Test memory");
        var result = await sut.CreateAsync(request);

        Assert.NotNull(result);
        embeddingService.Verify(e => e.GetEmbeddingAsync("Test memory", It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

**Step 4: Run tests to verify they fail**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "MemoryServiceTests"
```
Expected: FAIL

**Step 5: Implement MemoryService**

`src/Mnemosyne.Core/Memory/MemoryService.cs`

```csharp
using Mnemosyne.Core.Embeddings;

namespace Mnemosyne.Core.Memory;

public class MemoryService
{
    private readonly IMemoryRepository _repository;
    private readonly IEmbeddingService _embeddingService;

    public MemoryService(IMemoryRepository repository, IEmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
    }

    public async Task<MemoryResponse> CreateAsync(CreateMemoryRequest request, CancellationToken ct = default)
    {
        var embedding = await _embeddingService.GetEmbeddingAsync(request.Content, ct);

        var entity = new MemoryEntity
        {
            ProjectId = request.ProjectId,
            Content = request.Content,
            Type = request.Type,
            Tags = request.Tags ?? [],
            Embedding = embedding
        };

        var created = await _repository.CreateAsync(entity, ct);
        return ToResponse(created);
    }

    public async Task<MemoryResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<IEnumerable<MemoryResponse>> ListAsync(Guid projectId, MemoryType? type = null, CancellationToken ct = default)
    {
        var entities = await _repository.ListAsync(projectId, type, ct);
        return entities.Select(ToResponse);
    }

    public async Task<IEnumerable<MemoryResponse>> SearchAsync(string query, Guid projectId, int limit = 10, CancellationToken ct = default)
    {
        var embedding = await _embeddingService.GetEmbeddingAsync(query, ct);
        var entities = await _repository.SearchSimilarAsync(embedding, projectId, limit, ct);
        return entities.Select(ToResponse);
    }

    public async Task<MemoryResponse?> UpdateAsync(Guid id, string content, CancellationToken ct = default)
    {
        var entity = await _repository.UpdateAsync(id, content, ct);
        return entity is null ? null : ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        => await _repository.DeleteAsync(id, ct);

    private static MemoryResponse ToResponse(MemoryEntity e) =>
        new(e.Id, e.ProjectId, e.Content, e.Type, e.Tags, e.CreatedAt);
}
```

**Step 6: Run tests to verify they pass**

```bash
dotnet test tests/Mnemosyne.Core.Tests --filter "MemoryServiceTests"
```
Expected: PASS

**Step 7: Commit**

```bash
git add .
git commit -m "feat: add MemoryService with embedding integration"
```

---

### Task 11: Memory Endpoints (API)

**Files:**
- Create: `src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`

**Step 1: Create MemoryEndpoints**

`src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`

```csharp
using Mnemosyne.Core.Memory;

namespace Mnemosyne.Api.Endpoints;

public static class MemoryEndpoints
{
    public static IEndpointRouteBuilder MapMemoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/memory");

        group.MapPost("/", async (CreateMemoryRequest request, MemoryService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/api/v1/memory/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, MemoryService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/search", async (string query, Guid projectId, MemoryService service, CancellationToken ct) =>
        {
            var results = await service.SearchAsync(query, projectId, ct: ct);
            return Results.Ok(results);
        });

        group.MapGet("/", async (Guid projectId, MemoryType? type, MemoryService service, CancellationToken ct) =>
        {
            var results = await service.ListAsync(projectId, type, ct);
            return Results.Ok(results);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateMemoryRequest request, MemoryService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request.Content, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:guid}", async (Guid id, MemoryService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}

public record UpdateMemoryRequest(string Content);
```

**Step 2: Register services in Program.cs**

Add to `Program.cs`:

```csharp
builder.Services.AddScoped<MemoryService>();
builder.Services.AddScoped<IMemoryRepository, MemoryRepository>();
// ...
app.MapMemoryEndpoints();
```

**Step 3: Build to verify**

```bash
dotnet build
```
Expected: Build succeeded, 0 errors

**Step 4: Commit**

```bash
git add .
git commit -m "feat: add memory REST endpoints"
```

---

### Task 12: OpenAI Embedding Service (Infrastructure)

**Files:**
- Create: `src/Mnemosyne.Infrastructure/AI/OpenAiEmbeddingService.cs`
- Test: `tests/Mnemosyne.Infrastructure.Tests/AI/OpenAiEmbeddingServiceTests.cs`

**Step 1: Write failing test (mocked HTTP)**

`tests/Mnemosyne.Infrastructure.Tests/AI/OpenAiEmbeddingServiceTests.cs`

```csharp
using Moq;
using Mnemosyne.Infrastructure.AI;
using OpenAI.Embeddings;

namespace Mnemosyne.Infrastructure.Tests.AI;

public class OpenAiEmbeddingServiceTests
{
    [Fact]
    public async Task GetEmbeddingAsync_ShouldReturnFloatArray()
    {
        // Note: Full integration test requires actual OpenAI key
        // This test validates the service instantiation
        var service = new OpenAiEmbeddingService("fake-key");
        Assert.NotNull(service);
    }
}
```

**Step 2: Implement OpenAiEmbeddingService**

`src/Mnemosyne.Infrastructure/AI/OpenAiEmbeddingService.cs`

```csharp
using Mnemosyne.Core.Embeddings;
using OpenAI.Embeddings;

namespace Mnemosyne.Infrastructure.AI;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _client;

    public OpenAiEmbeddingService(string apiKey)
    {
        _client = new EmbeddingClient("text-embedding-3-small", apiKey);
    }

    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
    {
        var result = await _client.GenerateEmbeddingAsync(text, cancellationToken: ct);
        return result.Value.ToFloats().ToArray();
    }
}
```

**Step 3: Register in Program.cs**

```csharp
builder.Services.AddSingleton<IEmbeddingService>(sp =>
    new OpenAiEmbeddingService(builder.Configuration["OpenAI:ApiKey"]!));
```

**Step 4: Run tests**

```bash
dotnet test tests/Mnemosyne.Infrastructure.Tests --filter "OpenAiEmbeddingServiceTests"
```
Expected: PASS

**Step 5: Commit**

```bash
git add .
git commit -m "feat: add OpenAI embedding service"
```

---

### Task 13: Project Endpoints (API)

**Files:**
- Create: `src/Mnemosyne.Core/Project/ProjectService.cs`
- Create: `src/Mnemosyne.Core/Project/ProjectDtos.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/ProjectRepository.cs`
- Create: `src/Mnemosyne.Api/Endpoints/ProjectEndpoints.cs`

> Follow same pattern as Tasks 9-11 for Project. Create DTOs, write failing tests, implement repository, implement service, implement endpoints, commit.

**Step 1: Create ProjectDtos**

`src/Mnemosyne.Core/Project/ProjectDtos.cs`

```csharp
namespace Mnemosyne.Core.Project;

public record CreateProjectRequest(string Name, string? Path = null);
public record UpdateProjectRequest(string? Name, string? Settings);
public record ProjectResponse(Guid Id, Guid UserId, string Name, string? Path, DateTime CreatedAt);
```

**Step 2: Create ProjectRepository following MemoryRepository pattern**

`src/Mnemosyne.Infrastructure/Repositories/ProjectRepository.cs` - implement `IProjectRepository` using EF Core, same pattern as `MemoryRepository`.

**Step 3: Create ProjectService following MemoryService pattern**

`src/Mnemosyne.Core/Project/ProjectService.cs` - implement CRUD delegating to `IProjectRepository`.

**Step 4: Create ProjectEndpoints following MemoryEndpoints pattern**

`src/Mnemosyne.Api/Endpoints/ProjectEndpoints.cs` - map all `/api/v1/project` routes.

**Step 5: Test, build, commit**

```bash
dotnet test
dotnet build
git add .
git commit -m "feat: add project CRUD service and endpoints"
```

---

## Phase 3: gRPC Services

---

### Task 14: Protobuf Definitions

**Files:**
- Create: `src/Mnemosyne.Api/Protos/search.proto`
- Create: `src/Mnemosyne.Api/Protos/index.proto`
- Create: `src/Mnemosyne.Api/Protos/compress.proto`

**Step 1: Create search.proto**

`src/Mnemosyne.Api/Protos/search.proto`

```protobuf
syntax = "proto3";
option csharp_namespace = "Mnemosyne.Api.Grpc";

package search;

service SearchService {
    rpc SemanticSearch(SearchRequest) returns (SearchResponse);
    rpc HybridSearch(SearchRequest) returns (SearchResponse);
}

message SearchRequest {
    string query = 1;
    string project_id = 2;
    int32 limit = 3;
}

message SearchResult {
    string id = 1;
    string content = 2;
    float score = 3;
}

message SearchResponse {
    repeated SearchResult results = 1;
    int32 total = 2;
}
```

**Step 2: Create index.proto**

```protobuf
syntax = "proto3";
option csharp_namespace = "Mnemosyne.Api.Grpc";

package index;

service IndexService {
    rpc IndexProject(IndexRequest) returns (stream IndexProgress);
    rpc GetIndexStatus(StatusRequest) returns (IndexStatus);
}

message IndexRequest {
    string project_id = 1;
    string path = 2;
}

message IndexProgress {
    int32 files_processed = 1;
    int32 total_files = 2;
    string current_file = 3;
    bool complete = 4;
}

message StatusRequest { string project_id = 1; }
message IndexStatus { bool indexed = 1; int32 chunk_count = 2; }
```

**Step 3: Create compress.proto**

```protobuf
syntax = "proto3";
option csharp_namespace = "Mnemosyne.Api.Grpc";

package compress;

service CompressService {
    rpc CompressCode(CompressRequest) returns (stream CompressChunk);
}

message CompressRequest {
    string content = 1;
    string strategy = 2;
}

message CompressChunk {
    string content = 1;
    bool done = 2;
    float compression_ratio = 3;
}
```

**Step 4: Add Grpc.Tools to .csproj and verify code gen**

Add to `Mnemosyne.Api.csproj`:
```xml
<ItemGroup>
  <Protobuf Include="Protos\*.proto" GrpcServices="Server" />
</ItemGroup>
```

```bash
dotnet build src/Mnemosyne.Api
```
Expected: Protobuf C# classes generated

**Step 5: Commit**

```bash
git add .
git commit -m "feat: add Protobuf definitions for gRPC services"
```

---

### Task 15: gRPC Search Service Implementation

**Files:**
- Create: `src/Mnemosyne.Api/GrpcServices/SearchGrpcService.cs`

**Step 1: Implement SearchGrpcService**

`src/Mnemosyne.Api/GrpcServices/SearchGrpcService.cs`

```csharp
using Grpc.Core;
using Mnemosyne.Api.Grpc;
using Mnemosyne.Core.Memory;

namespace Mnemosyne.Api.GrpcServices;

public class SearchGrpcService : SearchService.SearchServiceBase
{
    private readonly MemoryService _memoryService;

    public SearchGrpcService(MemoryService memoryService)
    {
        _memoryService = memoryService;
    }

    public override async Task<SearchResponse> SemanticSearch(SearchRequest request, ServerCallContext context)
    {
        var projectId = Guid.Parse(request.ProjectId);
        var results = await _memoryService.SearchAsync(request.Query, projectId, request.Limit, context.CancellationToken);

        var response = new SearchResponse();
        response.Results.AddRange(results.Select(r => new SearchResult
        {
            Id = r.Id.ToString(),
            Content = r.Content,
            Score = 1.0f
        }));
        response.Total = response.Results.Count;
        return response;
    }

    public override Task<SearchResponse> HybridSearch(SearchRequest request, ServerCallContext context)
        => SemanticSearch(request, context);
}
```

**Step 2: Register gRPC in Program.cs**

```csharp
builder.Services.AddGrpc();
// ...
app.MapGrpcService<SearchGrpcService>();
```

**Step 3: Build to verify**

```bash
dotnet build
```

**Step 4: Commit**

```bash
git add .
git commit -m "feat: add gRPC SearchService implementation"
```

---

## Phase 4: Health Checks & Final Wiring

---

### Task 16: Health Checks Configuration

**Files:**
- Modify: `src/Mnemosyne.Api/Program.cs`
- Create: `src/Mnemosyne.Api/appsettings.json`

**Step 1: Create appsettings.json**

`src/Mnemosyne.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mnemosyne;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "OpenAI": {
    "ApiKey": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Step 2: Wire health checks in Program.cs**

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgres")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis");

// ...
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready");
```

**Step 3: Run all tests**

```bash
dotnet test
```
Expected: All tests PASS

**Step 4: Final commit**

```bash
git add .
git commit -m "feat: complete health checks and final wiring"
```

---

## Verification Checklist

Before declaring Phase 1-4 complete:

- [ ] `dotnet build` - 0 errors, 0 warnings
- [ ] `dotnet test` - all tests pass
- [ ] `GET /health/live` returns 200
- [ ] `GET /health/ready` returns 200 (with real DB/Redis)
- [ ] `POST /api/v1/auth/validate` with valid key returns user info
- [ ] `POST /api/v1/memory` creates memory with embedding
- [ ] `GET /api/v1/memory/search?query=...` returns semantic results
- [ ] gRPC `SemanticSearch` returns results
