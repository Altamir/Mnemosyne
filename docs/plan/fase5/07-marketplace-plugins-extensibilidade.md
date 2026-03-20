# Task 07 - Marketplace de plugins e extensibilidade

## Objetivo
Criar base de extensibilidade para conectores/plugins de terceiros com validacao e sandbox.

## Dependencias
- Requer Task 01 e Task 02 da Fase 5 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/PluginManifestEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Plugins/RegisterPlugin/RegisterPluginCommand.cs`
- Create: `src/Mnemosyne.Application/Features/Plugins/EnablePlugin/EnablePluginCommand.cs`
- Create: `src/Mnemosyne.Api/Endpoints/PluginEndpoints.cs`
- Create: `docs/plugins/plugin-manifest-spec.md`
- Create: `docs/plugins/plugin-security-guidelines.md`
- Test: `tests/Mnemosyne.UnitTests/Application/Plugins/PluginRegistrationTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Plugins/PluginEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para registro/habilitacao com validacao de manifesto.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar entidades, comandos e endpoints de plugins.
4. VERIFY GREEN: executar testes unitarios e integracao.
5. REFACTOR: separar validacoes de manifesto e politicas de seguranca.
6. DOCS: atualizar logs e ADR de arquitetura de extensibilidade.
7. COMMIT: `feat: adiciona base de marketplace de plugins`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "PluginRegistrationTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "PluginEndpointsTests"
```
