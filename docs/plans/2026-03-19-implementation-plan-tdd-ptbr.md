# Mnemosyne API Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Entregar a API v1 do Mnemosyne em .NET 10 com ingestao e busca semantica de memorias usando PostgreSQL 17.9 + pgvector 0.8.2, com TDD estrito e documentacao continua.

**Architecture:** Monolito modular em quatro projetos (`Mnemosyne.Api`, `Mnemosyne.Application`, `Mnemosyne.Domain`, `Mnemosyne.Infrastructure`) com fluxo HTTP -> Application -> Repository -> PostgreSQL. O acesso a dados usa EF Core para CRUD e Dapper para busca vetorial. Cada tarefa segue RED -> GREEN -> REFACTOR -> DOCS -> COMMIT.

**Tech Stack:** .NET 10, ASP.NET Core 10, EF Core 10, Dapper, Npgsql, Pgvector.EntityFrameworkCore, PostgreSQL 17.9, pgvector 0.8.2, xUnit, Moq, AutoFixture, Testcontainers.

---

## Regras obrigatorias (execucao)

- Codigo-fonte e testes em ingles (nomes de classes, metodos, variaveis, mensagens internas).
- Documentacao em pt-BR (`docs/**/*.md`, ADRs, logs de aprendizado, logs de erro/fix).
- Nenhuma implementacao sem teste falhando antes.
- Cada task deve terminar com atualizacao de documentacao continua:
  - `docs/implementation/learning-log.md`
  - `docs/implementation/error-fix-log.md`
  - `docs/decisions/ADR-xxxx-*.md` (quando houver decisao tecnica)

---

### Task 1: Bootstrap da solucao com .slnx

**Files:**
- Create: `Mnemosyne.slnx`
- Create: `src/Mnemosyne.Api/Mnemosyne.Api.csproj`
- Create: `src/Mnemosyne.Application/Mnemosyne.Application.csproj`
- Create: `src/Mnemosyne.Domain/Mnemosyne.Domain.csproj`
- Create: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Create: `tests/Mnemosyne.UnitTests/Mnemosyne.UnitTests.csproj`
- Create: `tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj`
- Create: `docs/implementation/learning-log.md`
- Create: `docs/implementation/error-fix-log.md`

**Step 1: Write the failing test**

`tests/Mnemosyne.UnitTests/SolutionStructureTests.cs`

```csharp
namespace Mnemosyne.UnitTests;

public class SolutionStructureTests
{
    [Fact(DisplayName = "Estrutura inicial de testes deve executar")]
    [Trait("Layer", "Domain - Utils")]
    public void Baseline_Executed_ShouldPass()
    {
        // Arrange

        // Act
        var result = true;

        // Assert
        Assert.True(result);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.UnitTests`
Expected: FAIL (projeto ainda nao criado)

**Step 3: Write minimal implementation**

```bash
dotnet new slnx -n Mnemosyne
dotnet new webapi -n Mnemosyne.Api -o src/Mnemosyne.Api -f net10.0 --use-minimal-apis
dotnet new classlib -n Mnemosyne.Application -o src/Mnemosyne.Application -f net10.0
dotnet new classlib -n Mnemosyne.Domain -o src/Mnemosyne.Domain -f net10.0
dotnet new classlib -n Mnemosyne.Infrastructure -o src/Mnemosyne.Infrastructure -f net10.0
dotnet new xunit -n Mnemosyne.UnitTests -o tests/Mnemosyne.UnitTests -f net10.0
dotnet new xunit -n Mnemosyne.IntegrationTests -o tests/Mnemosyne.IntegrationTests -f net10.0
dotnet slnx add src/Mnemosyne.Api/Mnemosyne.Api.csproj
dotnet slnx add src/Mnemosyne.Application/Mnemosyne.Application.csproj
dotnet slnx add src/Mnemosyne.Domain/Mnemosyne.Domain.csproj
dotnet slnx add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
dotnet slnx add tests/Mnemosyne.UnitTests/Mnemosyne.UnitTests.csproj
dotnet slnx add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj reference src/Mnemosyne.Application/Mnemosyne.Application.csproj src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
dotnet add src/Mnemosyne.Application/Mnemosyne.Application.csproj reference src/Mnemosyne.Domain/Mnemosyne.Domain.csproj
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj reference src/Mnemosyne.Domain/Mnemosyne.Domain.csproj src/Mnemosyne.Application/Mnemosyne.Application.csproj
dotnet add tests/Mnemosyne.UnitTests/Mnemosyne.UnitTests.csproj reference src/Mnemosyne.Domain/Mnemosyne.Domain.csproj src/Mnemosyne.Application/Mnemosyne.Application.csproj
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj reference src/Mnemosyne.Api/Mnemosyne.Api.csproj
mkdir -p docs/implementation docs/decisions
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.UnitTests`
Expected: PASS

**Step 5: Refactor**

- Organizar pastas base de features e persistencia:
  - `src/Mnemosyne.Application/Features/`
  - `src/Mnemosyne.Domain/Entities/`
  - `src/Mnemosyne.Infrastructure/Persistence/`

**Step 6: Documentacao continua**

- Atualizar `docs/implementation/learning-log.md` com aprendizados do bootstrap.
- Atualizar `docs/implementation/error-fix-log.md` com erros de setup e correcoes.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: inicializa solucao .NET 10 com arquivo slnx"
```

---

### Task 2: Modelo de dominio de memoria

**Files:**
- Create: `src/Mnemosyne.Domain/Entities/MemoryEntity.cs`
- Create: `src/Mnemosyne.Domain/Enums/MemoryType.cs`
- Test: `tests/Mnemosyne.UnitTests/Domain/MemoryEntityTests.cs`

**Step 1: Write the failing test**

```csharp
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.UnitTests.Domain;

public class MemoryEntityTests
{
    [Fact(DisplayName = "Deve criar memoria valida")]
    [Trait("Layer", "Domain - Entities")]
    public void ValidMemory_Executed_ShouldCreate()
    {
        // Arrange
        var content = "decision content";
        var type = MemoryType.Decision;

        // Act
        var entity = MemoryEntity.Create(Guid.NewGuid(), content, type, ["tag-a"]);

        // Assert
        Assert.Equal(content, entity.Content);
        Assert.Equal(type, entity.Type);
    }

    [Fact(DisplayName = "Nao deve criar memoria com conteudo vazio")]
    [Trait("Layer", "Domain - Entities")]
    public void EmptyContent_Executed_ShouldThrow()
    {
        // Arrange

        // Act
        Action act = () => MemoryEntity.Create(Guid.NewGuid(), "", MemoryType.Note, []);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "MemoryEntityTests"`
Expected: FAIL (tipos inexistentes)

**Step 3: Write minimal implementation**

```csharp
namespace Mnemosyne.Domain.Enums;

public enum MemoryType
{
    Note,
    Decision,
    Preference,
    Context
}
```

```csharp
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Domain.Entities;

public class MemoryEntity
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Content { get; private set; }
    public MemoryType Type { get; private set; }
    public string[] Tags { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private MemoryEntity(Guid id, Guid projectId, string content, MemoryType type, string[] tags)
    {
        Id = id;
        ProjectId = projectId;
        Content = content;
        Type = type;
        Tags = tags;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static MemoryEntity Create(Guid projectId, string content, MemoryType type, string[] tags)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        return new MemoryEntity(Guid.NewGuid(), projectId, content.Trim(), type, tags ?? []);
    }
}
```

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "MemoryEntityTests"`
Expected: PASS

**Step 5: Refactor**

- Extrair helper `CreateValidEntity` nos testes para reduzir duplicacao.

**Step 6: Documentacao continua**

- Criar `docs/decisions/ADR-0001-modelo-de-memoria.md` com contexto, alternativas e decisao.
- Atualizar learning e error-fix logs.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: adiciona modelo de dominio de memoria"
```

---

### Task 3: Persistencia PostgreSQL 17.9 + pgvector 0.8.2

**Files:**
- Modify: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Create: `src/Mnemosyne.Infrastructure/Persistence/MnemosyneDbContext.cs`
- Create: `src/Mnemosyne.Infrastructure/Persistence/Configurations/MemoryEntityConfiguration.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Persistence/DbContextMappingTests.cs`

**Step 1: Write the failing test**

```csharp
using Microsoft.EntityFrameworkCore;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.IntegrationTests.Persistence;

public class DbContextMappingTests
{
    [Fact(DisplayName = "Mapeamento de memoria deve expor tabela memories")]
    [Trait("Layer", "Infrastructure - Repository")]
    public void Mapping_Executed_ShouldContainMemoriesTable()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseInMemoryDatabase("mapping-test")
            .Options;

        // Act
        using var context = new MnemosyneDbContext(options);
        var model = context.Model;

        // Assert
        Assert.NotNull(model.FindEntityType("Mnemosyne.Domain.Entities.MemoryEntity"));
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "DbContextMappingTests"`
Expected: FAIL (DbContext inexistente)

**Step 3: Write minimal implementation**

```bash
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package Pgvector.EntityFrameworkCore
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj package Microsoft.EntityFrameworkCore.InMemory
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj reference src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj
```

Implementar DbContext + configuration com coluna `vector(1536)`.

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "DbContextMappingTests"`
Expected: PASS

**Step 5: Refactor**

- Centralizar nomes de tabela/schema em constantes internas de persistencia.

**Step 6: Documentacao continua**

- Atualizar learning-log com detalhes de versao PG/pgvector.
- Registrar erro comum de pacote/compatibilidade no error-fix-log.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: configura persistencia com postgresql e pgvector"
```

---

### Task 4: Migrations e inicializacao do banco

**Files:**
- Create: `src/Mnemosyne.Infrastructure/Persistence/Migrations/*`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Persistence/MigrationTests.cs`

**Step 1: Write the failing test**

```csharp
namespace Mnemosyne.IntegrationTests.Persistence;

public class MigrationTests
{
    [Fact(DisplayName = "Migration inicial deve existir")]
    [Trait("Layer", "Infrastructure - Repository")]
    public void InitialMigration_Executed_ShouldExist()
    {
        // Arrange

        // Act
        var migrationExists = Directory
            .EnumerateFiles("src/Mnemosyne.Infrastructure/Persistence/Migrations", "*Initial*.cs", SearchOption.AllDirectories)
            .Any();

        // Assert
        Assert.True(migrationExists);
    }
}
```

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "MigrationTests"`
Expected: FAIL (migration nao existe)

**Step 3: Write minimal implementation**

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialMemorySchema --project src/Mnemosyne.Infrastructure --startup-project src/Mnemosyne.Api
```

Adicionar no startup aplicacao de migration em ambiente local/dev.

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "MigrationTests"`
Expected: PASS

**Step 5: Refactor**

- Extrair extensao `ApplyDatabaseMigrations()` para limpar `Program.cs`.

**Step 6: Documentacao continua**

- Documentar fluxo de migrations no learning-log.
- Registrar erros de permissao/conexao no error-fix-log.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: adiciona migration inicial e bootstrap de banco"
```

---

### Task 5: Fluxo CreateMemory (TDD completo)

**Files:**
- Create: `src/Mnemosyne.Application/Features/Memory/CreateMemory/CreateMemoryCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Memory/CreateMemory/CreateMemoryHandler.cs`
- Create: `src/Mnemosyne.Domain/Interfaces/IMemoryRepository.cs`
- Create: `src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/CreateMemoryHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/CreateMemoryEndpointTests.cs`

**Step 1: Write the failing test**

Escrever testes unitarios para:
- criar memoria valida;
- rejeitar content vazio.

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "CreateMemoryHandlerTests"`
Expected: FAIL

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "CreateMemoryHandlerTests" -v n`
Expected: FAIL com tipos/handler inexistentes

**Step 3: Write minimal implementation**

Implementar command + handler + interface de repositorio + endpoint `POST /api/v1/memory`.

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "CreateMemoryHandlerTests"`
Expected: PASS

**Step 5: Refactor**

- Reduzir duplicacao entre mapping de request e command.
- Melhorar nomes e coesao sem alterar comportamento.

**Step 6: Documentacao continua**

- Atualizar learning-log com contrato final do endpoint.
- Se houver decisao de payload, criar `docs/decisions/ADR-0002-contrato-create-memory.md`.
- Atualizar error-fix-log com problemas encontrados.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: implementa endpoint de criacao de memoria"
```

---

### Task 6: Fluxo SearchMemory com pgvector (TDD completo)

**Files:**
- Create: `src/Mnemosyne.Application/Features/Memory/SearchMemory/SearchMemoryQuery.cs`
- Create: `src/Mnemosyne.Application/Features/Memory/SearchMemory/SearchMemoryHandler.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/MemoryRepository.cs`
- Modify: `src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/SearchMemoryHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/SearchMemoryEndpointTests.cs`

**Step 1: Write the failing test**

Testes devem garantir:
- ordenacao por similaridade;
- respeito ao limite (`topK`);
- retorno vazio quando nao houver match.

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "SearchMemoryHandlerTests"`
Expected: FAIL

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "SearchMemoryHandlerTests" -v n`
Expected: FAIL por ausencia de query/handler

**Step 3: Write minimal implementation**

Implementar query + handler + busca vetorial no repositorio e endpoint `GET /api/v1/memory/search`.

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.UnitTests --filter "SearchMemoryHandlerTests"`
Expected: PASS

**Step 5: Refactor**

- Extrair composicao de query vetorial para metodo dedicado.
- Garantir legibilidade e evitar SQL duplicado.

**Step 6: Documentacao continua**

- Criar `docs/decisions/ADR-0003-metrica-de-similaridade-vetorial.md`.
- Atualizar learning-log com benchmark inicial.
- Atualizar error-fix-log com tuning/casos limite.

**Step 7: Commit**

```bash
git add .
git commit -m "feat: adiciona busca semantica com pgvector"
```

---

### Task 7: Testes de integracao ponta a ponta

**Files:**
- Create: `tests/Mnemosyne.IntegrationTests/Fixtures/PostgresFixture.cs`
- Create: `tests/Mnemosyne.IntegrationTests/Api/MemoryE2ETests.cs`
- Create: `docker-compose.yml`
- Modify: `tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj`

**Step 1: Write the failing test**

Criar teste E2E:
1) cria memoria;
2) consulta busca semantica;
3) valida resposta e ordenacao.

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "MemoryE2ETests"`
Expected: FAIL

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "MemoryE2ETests" -v n`
Expected: FAIL por fixture/infra inexistente

**Step 3: Write minimal implementation**

```bash
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj package Testcontainers.PostgreSql
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj package Microsoft.AspNetCore.Mvc.Testing
```

Implementar fixture de PostgreSQL 17.9 e wiring de testes integrados.

**Step 4: Run test to verify it passes**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "MemoryE2ETests"`
Expected: PASS

**Step 5: Refactor**

- Consolidar helpers de seed e cleanup para reduzir flakiness.

**Step 6: Documentacao continua**

- Criar `docs/implementation/runbook-local.md` com execucao local de API + testes.
- Atualizar learning-log e error-fix-log com problemas de container/rede.

**Step 7: Commit**

```bash
git add .
git commit -m "test: cobre fluxo e2e com postgres em container"
```

---

### Task 8: Fechamento de qualidade + documentacao final

**Files:**
- Create: `README.md`
- Create: `docs/implementation/release-checklist-v1.md`
- Create: `docs/implementation/release-notes-v1.md`
- Modify: `src/Mnemosyne.Api/Program.cs`

**Step 1: Write the failing test**

Criar teste de health endpoint:
- `GET /health/live` retorna 200.

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "HealthEndpointTests"`
Expected: FAIL

**Step 2: Run test to verify it fails**

Run: `dotnet test tests/Mnemosyne.IntegrationTests --filter "HealthEndpointTests" -v n`
Expected: FAIL sem mapeamento de health check

**Step 3: Write minimal implementation**

Configurar health checks no `Program.cs` e mapear `/health/live` e `/health/ready`.

**Step 4: Run test to verify it passes**

Run: `dotnet test Mnemosyne.slnx -c Release`
Expected: PASS em toda suite

**Step 5: Refactor**

- Aplicar formatacao e limpeza final:

```bash
dotnet format Mnemosyne.slnx
dotnet build Mnemosyne.slnx -c Release
dotnet test Mnemosyne.slnx -c Release
```

**Step 6: Documentacao continua**

- Atualizar README com setup, comandos e troubleshooting.
- Atualizar `release-checklist-v1.md` e `release-notes-v1.md`.
- Consolidar aprendizados e top erros/fixes dos logs continuos.

**Step 7: Commit**

```bash
git add .
git commit -m "chore: finaliza baseline v1 com testes e documentacao"
```

---

## Checklist de validacao final

- [ ] Todos os fluxos implementados passaram por RED -> GREEN -> REFACTOR.
- [ ] Nenhuma implementacao foi criada sem teste falhando antes.
- [ ] `dotnet build Mnemosyne.slnx -c Release` sem erros.
- [ ] `dotnet test Mnemosyne.slnx -c Release` com todos os testes verdes.
- [ ] Endpoints validados por teste de integracao: create/search/health.
- [ ] `docs/implementation/learning-log.md` atualizado em todas as tasks.
- [ ] `docs/implementation/error-fix-log.md` atualizado em todas as tasks.
- [ ] ADRs criadas para decisoes relevantes (minimo 3).
