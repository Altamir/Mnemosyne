# Task 07 - Testes de integracao ponta a ponta

## Objetivo
Adicionar fixture de PostgreSQL containerizado e testes E2E cobrindo criacao e busca de memoria.

## Dependencias
- Requer Task 05 e Task 06 concluidas.

## Arquivos
- Create: `tests/Mnemosyne.IntegrationTests/Fixtures/PostgresFixture.cs`
- Create: `tests/Mnemosyne.IntegrationTests/Api/MemoryE2ETests.cs`
- Create: `docker-compose.yml`
- Modify: `tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj`

## Planejamento (TDD)
1. RED: escrever teste E2E (create -> search -> validacao de ranking).
2. VERIFY RED: executar e confirmar falha por falta da fixture/infra.
3. GREEN: configurar Testcontainers e fixture com PostgreSQL 17.9.
4. VERIFY GREEN: executar `MemoryE2ETests` e validar sucesso.
5. REFACTOR: consolidar helpers de seed e cleanup para reduzir flakiness.
6. DOCS: atualizar runbook local + learning/error-fix logs.
7. COMMIT: `test: cobre fluxo e2e com postgres em container`.

## Comandos principais
```bash
dotnet add tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj package Testcontainers.PostgreSql
dotnet test tests/Mnemosyne.IntegrationTests --filter "MemoryE2ETests"
```
