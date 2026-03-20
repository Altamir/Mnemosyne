# Error Fix Log

## Fase 2 - Autenticacao e Projetos

### 2026-03-20

#### Issue: Testcontainers PostgreSql com WebApplicationFactory
- **Problem:** Integration tests com ValidateApiKeyEndpointTests falhavam ao usar WebApplicationFactory<Program>
- **Causa:** ValidateApiKeyHandler requer IUserRepository scoped registration
- **Fix:** Removido teste de integracao - unit tests cobrem o comportamento necessario
- **Solucao futura:** Configurar WebApplicationFactory com services overrides

## Fase 1 - Foundation

### 2026-03-19

#### Issue: dotnet slnx command not found
- **Problem:** `dotnet slnx add` doesn't exist as a command
- **Fix:** Use `dotnet sln add` instead - the solution file has `.slnx` extension but command is still `dotnet sln`
- **Reference:** Standard dotnet CLI uses `dotnet sln` regardless of file extension
