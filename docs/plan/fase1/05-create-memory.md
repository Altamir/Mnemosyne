# Task 05 - Fluxo CreateMemory

## Objetivo
Implementar fluxo completo de criacao de memoria (command/handler/repository/endpoint) com cobertura de testes.

## Dependencias
- Requer Task 02 e Task 04 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Application/Features/Memory/CreateMemory/CreateMemoryCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Memory/CreateMemory/CreateMemoryHandler.cs`
- Create: `src/Mnemosyne.Domain/Interfaces/IMemoryRepository.cs`
- Create: `src/Mnemosyne.Api/Endpoints/MemoryEndpoints.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/CreateMemoryHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/CreateMemoryEndpointTests.cs`

## Planejamento (TDD)
1. RED: criar testes unitarios do handler (sucesso e validacao de input).
2. VERIFY RED: executar testes e confirmar falha por ausencia do fluxo.
3. GREEN: implementar command + handler + contrato de repositorio + endpoint POST.
4. VERIFY GREEN: executar testes unitarios e de integracao do endpoint.
5. REFACTOR: limpar mapping request->command e reduzir duplicacao.
6. DOCS: atualizar logs e ADR de contrato, se houver decisao relevante.
7. COMMIT: `feat: implementa endpoint de criacao de memoria`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "CreateMemoryHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "CreateMemoryEndpointTests"
```
