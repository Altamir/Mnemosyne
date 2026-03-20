# Task 03 - Indexacao assincrona de projeto

## Objetivo
Implementar fluxo de indexacao assincrona por projeto com status de processamento.

## Dependencias
- Requer Task 02 da Fase 2 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/ProjectIndexJobEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Index/StartProjectIndex/StartProjectIndexCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Index/StartProjectIndex/StartProjectIndexHandler.cs`
- Create: `src/Mnemosyne.Application/Features/Index/GetIndexStatus/GetIndexStatusQuery.cs`
- Create: `src/Mnemosyne.Application/Features/Index/GetIndexStatus/GetIndexStatusHandler.cs`
- Create: `src/Mnemosyne.Infrastructure/Services/ProjectIndexerService.cs`
- Modify: `src/Mnemosyne.Api/Endpoints/ProjectEndpoints.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Index/StartProjectIndexHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Index/ProjectIndexEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para iniciar indexacao e consultar status.
2. VERIFY RED: executar e validar falha.
3. GREEN: implementar command/query handlers e endpoint `/api/v1/project/:id/index`.
4. VERIFY GREEN: executar testes e validar status transitions.
5. REFACTOR: isolar logica de fila/processamento em servico dedicado.
6. DOCS: atualizar learning/error-fix logs e ADR sobre modelo assíncrono.
7. COMMIT: `feat: adiciona indexacao assincrona por projeto`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "StartProjectIndexHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "ProjectIndexEndpointsTests"
```
