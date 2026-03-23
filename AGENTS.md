# AGENTS.md

Mnemosyne -- semantic memory management system with vector indexing.

## Stack

- **.NET 10** (C#), Minimal APIs, EF Core + Npgsql
- **PostgreSQL 18 + pgvector 0.8.2** for vector persistence
- **xUnit + Moq + AutoFixture** for tests, **Testcontainers** for integration tests
- No MediatR -- plain handler classes with manual DI registration

## Project Structure

```
src/
  Mnemosyne.Api/              # Endpoints, Middleware, Program.cs
  Mnemosyne.Application/      # Features/{Domain}/{Action}/ (Commands, Queries, Handlers)
  Mnemosyne.Domain/           # Entities, Interfaces, Enums, Services
  Mnemosyne.Infrastructure/   # Repositories, Persistence, AI, Compression
tests/
  Mnemosyne.UnitTests/        # Unit tests (mirrors Application/Domain/Infrastructure)
  Mnemosyne.IntegrationTests/ # Integration tests with Testcontainers (PostgreSQL)
```

Solution file: `Mnemosyne.slnx` (XML-based `.slnx` format, not legacy `.sln`).

## Build & Test Commands

```bash
dotnet build                                    # Build all projects
dotnet test                                     # Run all tests
dotnet test tests/Mnemosyne.UnitTests           # Unit tests only
dotnet test tests/Mnemosyne.IntegrationTests    # Integration tests only

# Run a single test by fully-qualified name
dotnet test --filter "FullyQualifiedName~CreateMemoryHandlerTests.ValidContent_Executed_ReturnsCreatedMemory"

# Run tests by class name
dotnet test --filter "ClassName~CreateMemoryHandlerTests"

# Run tests by Trait
dotnet test --filter "Layer=Application - Commands"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report (requires reportgenerator tool)
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coverage"
```

Minimum coverage threshold: **70%** line coverage.

## Infrastructure

```bash
docker compose up -d    # Start PostgreSQL with pgvector
# Connection: Host=localhost;Database=mnemosyne;Username=postgres;Password=postgres
```

## Language Conventions

| Context               | Language |
|-----------------------|----------|
| Code (classes, methods, variables) | English |
| Commits               | pt-BR (Conventional Commits) |
| Documentation (XML, README, .md)   | pt-BR |
| User-facing error messages          | pt-BR |
| System logs           | English |

## Naming Conventions

| Type         | Pattern                        | Example                     |
|--------------|--------------------------------|-----------------------------|
| Command      | `{Action}{Entity}Command`      | `CreateMemoryCommand`       |
| Query        | `{Action}{Entity}Query`        | `SearchMemoryQuery`         |
| Handler      | `{Action}{Entity}Handler`      | `CreateMemoryHandler`       |
| Repository   | `I{Entity}Repository` / `{Entity}Repository` | `IMemoryRepository` |
| Endpoint     | `{Entity}Endpoints`            | `MemoryEndpoints`           |
| Entity       | `{Entity}Entity`               | `MemoryEntity`              |
| Test class   | `{ClassUnderTest}Tests`        | `CreateMemoryHandlerTests`  |
| Test method  | `{Condition}_Executed_{Result}`| `ValidContent_Executed_ReturnsCreatedMemory` |
| Private field| `_{camelCase}`                 | `_repositoryMock`           |

## Code Style

- **Nullable**: enabled globally, all projects
- **Implicit usings**: enabled (no GlobalUsings.cs files)
- **File-scoped namespaces**: `namespace X.Y.Z;` (no braces)
- **Records for DTOs**: Commands/Queries are C# `record` types with primary constructors
- **No `.editorconfig` or analyzers** -- follow patterns in existing code
- **Entity pattern**: private constructor + static `Create()` factory, private setters, validation in factory
- **Handler pattern**: plain class with `Handle(TCommand, CancellationToken)` method, constructor-injected dependencies, inline validation (no FluentValidation)
- **Endpoint pattern**: static class with `Map{Entity}Endpoints(this WebApplication)` extension method, route groups at `/api/v1/{resource}`, request records colocated in same file
- **DI registration**: manual `AddScoped`/`AddSingleton` in `Program.cs` (no reflection-based scanning)
- **Error handling in domain**: throw `ArgumentException` for invalid arguments in entity factories
- **Error handling in handlers**: validate inputs, throw exceptions for business rule violations

## Test Conventions

- **Framework**: xUnit with `[Fact]` and `[Theory]`/`[InlineData]`
- **Display names**: pt-BR via `[Fact(DisplayName = "Descricao em portugues")]`
- **Traits**: `[Trait("Layer", "Application - Commands")]`, `[Trait("Layer", "Application - Queries")]`, `[Trait("Layer", "Infrastructure - Compression")]`
- **Structure**: AAA pattern with explicit `// Arrange`, `// Act`, `// Assert` comments (or `// Act & Assert`)
- **Mocking**: Moq -- `Mock<IInterface>`, `.Setup().ReturnsAsync()`, `.Verify(Times.Once/Never)`
- **Test data**: AutoFixture with recursion behavior configured in constructor:
  ```csharp
  _fixture = new Fixture();
  _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
  _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
  ```
- **Setup**: constructor-based (no `[SetUp]` or `IClassFixture` for unit tests)
- **Assertions**: xUnit `Assert.*` only (no FluentAssertions)
- **Integration tests**: Testcontainers with `pgvector/pgvector:0.8.2-pg18-trixie`, `IAsyncLifetime`

## Commit Format

```
<type>(<scope>): <description in pt-BR>

<optional body in pt-BR>
```

Types: `feat`, `fix`, `docs`, `test`, `chore`, `refactor`

Atomic commits -- one logical change per commit. Never mix code changes with documentation changes.

## Application Architecture

```
Endpoint -> Handler -> Repository -> Database
                    -> IEmbeddingService (for vector generation)
                    -> ICompressionStrategy (for context compression)
```

- No MediatR/CQRS library -- handlers are plain classes injected directly
- Commands/Queries are records in `Application/Features/{Domain}/{Action}/`
- Each feature folder contains its command/query record + handler class
- Repositories implement interfaces from `Domain/Interfaces/`
- EF Core configurations in `Infrastructure/Persistence/Configurations/`
- Database uses `mnemosyne` schema, snake_case table/column names, `vector(1536)` for embeddings

## Key Dependencies

| Package | Project | Purpose |
|---------|---------|---------|
| Pgvector + Pgvector.EntityFrameworkCore | Domain, Infrastructure | Vector type and EF Core integration |
| BCrypt.Net-Next | Domain | API key hashing |
| OpenAI (2.9.1) | Infrastructure | Embedding generation |
| Npgsql.EntityFrameworkCore.PostgreSQL | Infrastructure | PostgreSQL EF Core provider |
| Testcontainers.PostgreSql | IntegrationTests | Containerized test database |

## Development Workflow Skill

When executing roadmap tasks (phases/tasks from `docs/plan/`), load the project-specific skill:

```
.opencode/skills/mnemosyne-task-workflow/SKILL.md
```

This skill guides the full cycle: read task spec -> TDD (RED/GREEN/REFACTOR) -> documentation -> commit.

## Files to Ignore

```
bin/ obj/ Debug/ Release/ coverage/ coverage-results/ *.user *.suo .DS_Store
```
