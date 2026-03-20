# Task 01 - Autenticacao por API Key

## Objetivo
Implementar autenticacao por API Key com hash seguro, middleware e endpoint de validacao.

## Dependencias
- Requer Fase 1 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/UserEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Auth/ValidateApiKey/ValidateApiKeyQuery.cs`
- Create: `src/Mnemosyne.Application/Features/Auth/ValidateApiKey/ValidateApiKeyHandler.cs`
- Create: `src/Mnemosyne.Api/Middleware/ApiKeyMiddleware.cs`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.UnitTests/Application/Auth/ValidateApiKeyHandlerTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Auth/ValidateApiKeyEndpointTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de handler e endpoint para chave valida/invalida.
2. VERIFY RED: executar e confirmar falha esperada.
3. GREEN: implementar user model, handler, middleware e endpoint `/api/v1/auth/validate`.
4. VERIFY GREEN: executar unitarios + integracao e validar sucesso.
5. REFACTOR: limpar extracao de header/auth context.
6. DOCS: atualizar learning/error-fix logs e ADR de estrategia de API Key.
7. COMMIT: `feat: implementa autenticacao por api key`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj package BCrypt.Net-Next
dotnet test tests/Mnemosyne.UnitTests --filter "ValidateApiKeyHandlerTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "ValidateApiKeyEndpointTests"
```
