# Task 01 - Enterprise SSO e RBAC

## Objetivo
Adicionar autenticacao enterprise (OIDC/SAML via provider) e controle de acesso por papeis (RBAC).

## Dependencias
- Requer Fase 4 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/RoleEntity.cs`
- Create: `src/Mnemosyne.Domain/Entities/UserRoleEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Auth/SsoLogin/SsoLoginCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Auth/SsoLogin/SsoLoginHandler.cs`
- Create: `src/Mnemosyne.Api/Authorization/RolePolicies.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Auth/SsoLoginHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Security/RbacPoliciesTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de login SSO e autorizacao por role.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar fluxo SSO e policies de RBAC.
4. VERIFY GREEN: executar testes unitarios e integrados.
5. REFACTOR: consolidar mapeamento de claims para roles internas.
6. DOCS: atualizar logs e ADR de modelo de identidade enterprise.
7. COMMIT: `feat: adiciona sso e rbac enterprise`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "SsoLoginHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "RbacPoliciesTests"
```
