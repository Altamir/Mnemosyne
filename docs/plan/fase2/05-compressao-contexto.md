# Task 05 - Compressao de contexto

## Objetivo
Implementar compressao de contexto/codigo com estrategias configuraveis.

## Dependencias
- Requer Task 04 da Fase 2 concluida.

## Arquivos
- Create: `src/Mnemosyne.Application/Features/Compress/CompressContext/CompressContextCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Compress/CompressContext/CompressContextHandler.cs`
- Create: `src/Mnemosyne.Domain/Services/ICompressionStrategy.cs`
- Create: `src/Mnemosyne.Infrastructure/Compression/CodeStructureCompressionStrategy.cs`
- Create: `src/Mnemosyne.Api/Endpoints/CompressEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Compress/CompressContextHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Compress/CompressEndpointTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para estrategia default e formato de resposta.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar handlers/estrategias e endpoint `/api/v1/compress`.
4. VERIFY GREEN: executar unitarios + integracao do endpoint.
5. REFACTOR: separar estrategias e evitar if/switch excessivo.
6. DOCS: atualizar logs e ADR da estrategia de compressao default.
7. COMMIT: `feat: adiciona compressao de contexto`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "CompressContextHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "CompressEndpointTests"
```
