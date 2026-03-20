# Task 02 - CRUD de Projetos

## Objetivo
Implementar CRUD de projetos para organizar memórias por contexto e isolamento lógico.

## Dependencias
- Requer Task 01 da Fase 2 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/ProjectEntity.cs`
- Create: `src/Mnemosyne.Domain/Interfaces/IProjectRepository.cs`
- Create: `src/Mnemosyne.Application/Features/Project/CreateProject/CreateProjectCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Project/CreateProject/CreateProjectHandler.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/ProjectRepository.cs`
- Create: `src/Mnemosyne.Api/Endpoints/ProjectEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Project/CreateProjectHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Project/ProjectCrudEndpointTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de criacao/listagem e validacoes de input.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar entidade, repositorio e endpoints `/api/v1/project`.
4. VERIFY GREEN: executar testes unitarios e integrados.
5. REFACTOR: padronizar responses/erros do CRUD.
6. DOCS: atualizar learning/error-fix logs e ADR de isolamento por projeto.
7. COMMIT: `feat: adiciona crud de projetos`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "CreateProjectHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "ProjectCrudEndpointTests"
```
