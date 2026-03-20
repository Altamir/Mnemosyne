# Task 07 - Observabilidade e health checks detalhados

## Objetivo
Adicionar logging estruturado, correlacao basica e health checks detalhados (DB, Redis, OpenAI).

## Dependencias
- Requer Task 01, Task 04 e Task 06 da Fase 2 concluidas.

## Arquivos
- Modify: `src/Mnemosyne.Api/Program.cs`
- Modify: `src/Mnemosyne.Api/appsettings.json`
- Create: `src/Mnemosyne.Api/Configuration/HealthChecksConfig.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Health/DetailedHealthChecksTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para `/health/live` e `/health/ready` com dependencias reais/mockadas.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: configurar health checks detalhados e logging estruturado.
4. VERIFY GREEN: executar testes e validar status por dependencia.
5. REFACTOR: extrair configuracao para classe dedicada.
6. DOCS: atualizar logs e runbook de troubleshooting de observabilidade.
7. COMMIT: `feat: adiciona health checks detalhados e observabilidade basica`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj package AspNetCore.HealthChecks.NpgSql
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj package AspNetCore.HealthChecks.Redis
dotnet test tests/Mnemosyne.IntegrationTests --filter "DetailedHealthChecksTests"
```
