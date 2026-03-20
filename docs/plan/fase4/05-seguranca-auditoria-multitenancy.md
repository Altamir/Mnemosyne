# Task 05 - Seguranca, auditoria e multitenancy

## Objetivo
Fortalecer seguranca com auditoria completa e validacoes de isolamento multi-tenant.

## Dependencias
- Requer Task 01 e Task 02 da Fase 4 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/AuditLogEntity.cs`
- Create: `src/Mnemosyne.Domain/Interfaces/IAuditLogRepository.cs`
- Create: `src/Mnemosyne.Infrastructure/Repositories/AuditLogRepository.cs`
- Create: `src/Mnemosyne.Api/Middleware/TenantIsolationMiddleware.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Security/TenantIsolationTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Security/AuditLogTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para bloqueio de acesso cross-tenant e registro de auditoria.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar middleware de isolamento e persistencia de audit logs.
4. VERIFY GREEN: executar testes de seguranca e auditoria.
5. REFACTOR: consolidar enrich de contexto de usuario/tenant.
6. DOCS: atualizar logs e ADR de estrategia de multitenancy.
7. COMMIT: `feat: reforca seguranca com auditoria e isolamento tenant`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.IntegrationTests --filter "TenantIsolationTests|AuditLogTests"
```
