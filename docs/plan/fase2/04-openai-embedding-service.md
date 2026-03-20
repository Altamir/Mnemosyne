# Task 04 - Servico de embeddings (OpenAI)

## Objetivo
Implementar servico de embeddings com OpenAI e integracao ao pipeline de memoria/indexacao.

## Dependencias
- Requer Task 01 da Fase 2 concluida.

## Arquivos
- Create: `src/Mnemosyne.Application/Abstractions/IEmbeddingService.cs`
- Create: `src/Mnemosyne.Infrastructure/AI/OpenAiEmbeddingService.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Modify: `src/Mnemosyne.Application/Features/Memory/CreateMemory/CreateMemoryHandler.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Memory/CreateMemoryEmbeddingTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Infrastructure/AI/OpenAiEmbeddingServiceTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para geracao de embedding e propagacao ao fluxo de create.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar interface + servico OpenAI e registrar DI/config.
4. VERIFY GREEN: executar testes unitarios/integracao controlada.
5. REFACTOR: encapsular retries/timeouts e tratamento de erro externo.
6. DOCS: atualizar logs e ADR da escolha de provedor de embedding.
7. COMMIT: `feat: integra servico de embeddings com openai`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package OpenAI
dotnet test tests/Mnemosyne.UnitTests --filter "CreateMemoryEmbeddingTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "OpenAiEmbeddingServiceTests"
```
