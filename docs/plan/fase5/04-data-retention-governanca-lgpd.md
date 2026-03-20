# Task 04 - Data retention e governanca (LGPD)

## Objetivo
Implementar politicas de retencao, anonimização e exclusao de dados alinhadas a LGPD/compliance.

## Dependencias
- Requer Task 02 da Fase 5 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/DataRetentionPolicyEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Governance/ApplyRetentionPolicy/ApplyRetentionPolicyCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Governance/AnonymizeUserData/AnonymizeUserDataCommand.cs`
- Create: `src/Mnemosyne.Api/Endpoints/GovernanceEndpoints.cs`
- Create: `docs/compliance/politica-retencao-dados.md`
- Create: `docs/compliance/processo-atendimento-lgpd.md`
- Test: `tests/Mnemosyne.UnitTests/Application/Governance/RetentionPolicyTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Compliance/GovernanceEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para retencao, anonimização e exclusao segura.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar comandos e endpoints de governanca.
4. VERIFY GREEN: executar testes e validar trilhas de auditoria.
5. REFACTOR: consolidar utilitarios de mascaramento/anonimizacao.
6. DOCS: atualizar logs e ADR de politica de ciclo de vida de dados.
7. COMMIT: `feat: adiciona governanca de dados com foco em lgpd`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "RetentionPolicyTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "GovernanceEndpointsTests"
```
