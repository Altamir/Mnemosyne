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
- **Causa:** Multiplos problemas: (1) IEmbeddingService nao registrado no DI, (2) conflito dual-provider Npgsql vs InMemory, (3) coluna pgvector Vector nao suportada pelo InMemory provider, (4) handlers de Task 3 nao registrados causando falha de inferencia de parametros do Minimal API
- **Fix:** Criado MnemosyneWebAppFactory que: remove todos os descriptors EF Core/Npgsql, registra InMemory com callback para ignorar Embedding, registra StubEmbeddingService, e registra handlers ausentes no Program.cs
- **Aprendizado:** WebApplicationFactory requer substituicao completa do provider EF Core (nao basta remover DbContextOptions), e o Minimal API valida TODOS os endpoints na inicializacao (nao apenas os chamados)

#### Issue: MemoryEndpoints /search com inferencia de parametros invalida
- **Problem:** `SearchMemoryHandler handler = null!` como parametro default fazia ASP.NET Minimal API nao reconhecer como servico DI
- **Causa:** Parametros com valor default nao sao inferidos como `[FromServices]` pelo Minimal API
- **Fix:** Adicionado atributo `[FromServices]` explicito e removido valor default `= null!`
- **Referencia:** Minimal API infere servicos DI apenas para parametros sem default value

## Fase 1 - Foundation

### 2026-03-19

#### Issue: dotnet slnx command not found
- **Problem:** `dotnet slnx add` doesn't exist as a command
- **Fix:** Use `dotnet sln add` instead - the solution file has `.slnx` extension but command is still `dotnet sln`
- **Reference:** Standard dotnet CLI uses `dotnet sln` regardless of file extension
