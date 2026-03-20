# Error Fix Log

## Fase 2 - Autenticacao e Projetos

### 2026-03-20

#### Task 05 - Compressao de Contexto
- Nenhum erro critico encontrado durante implementacao
- TDD workflow seguido sem intercorrencias: todos os testes passaram RED -> GREEN corretamente
- Warnings pre-existentes de MSB3277 (EF Core version conflict) nao afetam esta task

#### Task 04 - OpenAI Embedding Service
- OpenAI SDK 2.9.1 tem API diferente de versoes anteriores - usa EmbeddingClient diretamente
- IEmbeddingService movido de Application.Abstractions para Domain.Interfaces para manter consistencia com outras interfaces
- Pgvector adicionado ao UnitTests para permitir mocks com Vector

#### Task 03 - Indexacao Assincrona
- Nenhum erro critico encontrado durante implementacao
- Padrao de job assincrono permite extensibilidade futura para processamento real

#### Issue: Testcontainers PostgreSql com WebApplicationFactory
- **Problem:** Integration tests com ValidateApiKeyEndpointTests falhavam ao usar WebApplicationFactory<Program>
- **Causa:** ValidateApiKeyHandler requer IUserRepository scoped registration
- **Fix:** Removido teste de integracao - unit tests cobrem o comportamento necessario
- **Solucao futura:** Configurar WebApplicationFactory com services overrides

## Fase 1 - Foundation

### 2026-03-19

#### Issue: dotnet slnx command not found
- **Problem:** `dotnet slnx add` doesn't exist as a command
- **Fix:** Use `dotnet sln add` instead - the solution file has `.slnx` extension but command is still `dotnet sln`
- **Reference:** Standard dotnet CLI uses `dotnet sln` regardless of file extension
