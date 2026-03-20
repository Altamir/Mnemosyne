# Task 06 - Fluxo SearchMemory com pgvector

## Objetivo
Implementar busca semantica por similaridade vetorial com suporte a limite e ordenacao.

## Dependencias
- Requer Task 03, Task 04 e Task 05 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Application/Features/Memory/SearchMemory/SearchMemoryQuery.cs`
- Create: `src/Mnemosyne.Application/Features/Memory/SearchMemory/SearchMemoryHandler.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/MemoryRepository.cs`
- Modify: `src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/SearchMemoryHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/SearchMemoryEndpointTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para ordenacao por similaridade, limite topK e resposta vazia.
2. VERIFY RED: executar testes e confirmar falha.
3. GREEN: implementar query/handler/repositorio e endpoint de busca.
4. VERIFY GREEN: executar testes unitarios e integrados do fluxo.
5. REFACTOR: isolar construcao de query vetorial e melhorar legibilidade.
6. DOCS: criar ADR da metrica de similaridade + atualizar logs.
7. COMMIT: `feat: adiciona busca semantica com pgvector`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "SearchMemoryHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "SearchMemoryEndpointTests"
```
