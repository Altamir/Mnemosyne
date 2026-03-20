# Task 08 - Fechamento de qualidade e documentacao

## Objetivo
Consolidar health checks, qualidade final (build/test/format) e documentacao de release.

## Dependencias
- Requer Task 07 concluida.

## Arquivos
- Create: `README.md`
- Create: `docs/implementation/release-checklist-v1.md`
- Create: `docs/implementation/release-notes-v1.md`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/HealthEndpointTests.cs`

## Planejamento (TDD)
1. RED: escrever teste para `/health/live` retornar 200.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar health checks e mapear endpoints.
4. VERIFY GREEN: executar suite completa em Release.
5. REFACTOR: aplicar formatacao e limpeza final.
6. DOCS: consolidar release checklist/notes e logs de aprendizado/fixes.
7. COMMIT: `chore: finaliza baseline v1 com testes e documentacao`.

## Comandos principais
```bash
dotnet format Mnemosyne.slnx
dotnet build Mnemosyne.slnx -c Release
dotnet test Mnemosyne.slnx -c Release
```
