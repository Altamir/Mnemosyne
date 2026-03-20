# Task 01 - Redis cache e rate limiting

## Objetivo
Adicionar camada de cache Redis para consultas frequentes e rate limiting por API Key.

## Dependencias
- Requer Fase 3 concluida.

## Arquivos
- Create: `src/Mnemosyne.Infrastructure/Cache/RedisCacheService.cs`
- Create: `src/Mnemosyne.Api/Middleware/RateLimitMiddleware.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Modify: `src/Mnemosyne.Api/appsettings.json`
- Test: `tests/Mnemosyne.UnitTests/Infrastructure/Cache/RedisCacheServiceTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/RateLimitTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para cache hit/miss e limites por chave.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar servico de cache e middleware de rate limiting.
4. VERIFY GREEN: executar testes unitarios e integrados.
5. REFACTOR: centralizar politicas de TTL e limites por plano.
6. DOCS: atualizar logs e ADR de politicas de cache/rate limit.
7. COMMIT: `feat: adiciona cache redis e rate limiting`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package StackExchange.Redis
dotnet test tests/Mnemosyne.UnitTests --filter "RedisCacheServiceTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "RateLimitTests"
```
