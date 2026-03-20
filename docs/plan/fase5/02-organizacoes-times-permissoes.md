# Task 02 - Organizacoes, times e permissoes

## Objetivo
Evoluir o modelo multi-tenant para organizacoes com times e permissoes granulares por recurso.

## Dependencias
- Requer Task 01 da Fase 5 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/OrganizationEntity.cs`
- Create: `src/Mnemosyne.Domain/Entities/TeamEntity.cs`
- Create: `src/Mnemosyne.Domain/Entities/PermissionEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Organizations/CreateOrganization/CreateOrganizationCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Organizations/AddTeamMember/AddTeamMemberCommand.cs`
- Create: `src/Mnemosyne.Api/Endpoints/OrganizationEndpoints.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Organizations/OrganizationPermissionTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Organizations/OrganizationEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de criacao de org/time e validacao de permissoes.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar entidades, handlers e endpoints de organizacao.
4. VERIFY GREEN: executar testes unitarios e de integracao.
5. REFACTOR: extrair regras de autorizacao para servico dedicado.
6. DOCS: atualizar logs e ADR de governanca de acesso por organizacao.
7. COMMIT: `feat: adiciona organizacoes times e permissoes granulares`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "OrganizationPermissionTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "OrganizationEndpointsTests"
```
