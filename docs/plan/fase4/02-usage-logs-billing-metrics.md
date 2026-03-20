# Task 02 - Usage logs, billing e metricas

## Objetivo
Implementar coleta estruturada de uso (tokens, latencia, endpoint) para billing e analytics.

## Dependencias
- Requer Task 01 da Fase 4 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/UsageLogEntity.cs`
- Create: `src/Mnemosyne.Domain/Interfaces/IUsageLogRepository.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/UsageLogRepository.cs`
- Create: `src/Mnemosyne.Api/Middleware/UsageLoggingMiddleware.cs`
- Create: `src/Mnemosyne.Api/Endpoints/UsageEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Usage/UsageAggregationTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Usage/UsageEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para persistencia e agregacao de uso.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar entidade/repositorio/middleware e endpoint `/api/v1/auth/usage`.
4. VERIFY GREEN: executar testes de agregacao e endpoint.
5. REFACTOR: otimizar consulta agregada por periodo/plano.
6. DOCS: atualizar logs e ADR do modelo de billing baseado em uso.
7. COMMIT: `feat: adiciona logs de uso e metricas para billing`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "UsageAggregationTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "UsageEndpointsTests"
```
