# Task 03 - Persistencia PostgreSQL 17.9 + pgvector 0.8.2

## Objetivo
Configurar camada de persistencia (`DbContext` + mapeamentos) com PostgreSQL e pgvector.

## Dependencias
- Requer Task 01 e Task 02 concluidas.

## Arquivos
- Modify: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Create: `src/Mnemosyne.Infrastructure/Persistence/MnemosyneDbContext.cs`
- Create: `src/Mnemosyne.Infrastructure/Persistence/Configurations/MemoryEntityConfiguration.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Persistence/DbContextMappingTests.cs`

## Planejamento (TDD)
1. RED: criar teste de mapping do `MemoryEntity` no `DbContext`.
2. VERIFY RED: executar teste e validar falha por ausencia de implementacao.
3. GREEN: instalar pacotes EF/Npgsql/Pgvector e implementar `DbContext` + configuration.
4. VERIFY GREEN: executar teste de mapeamento e validar sucesso.
5. REFACTOR: consolidar constantes de schema/tabela/dimensao.
6. DOCS: registrar setup PostgreSQL/pgvector e erros de compatibilidade.
7. COMMIT: `feat: configura persistencia com postgresql e pgvector`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package Pgvector.EntityFrameworkCore
dotnet test tests/Mnemosyne.IntegrationTests --filter "DbContextMappingTests"
```
