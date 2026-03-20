# Task 01 - Bootstrap da solucao com .slnx

## Objetivo
Criar a estrutura base da solucao em .NET 10 com `Mnemosyne.slnx`, projetos principais, projetos de teste e pastas de documentacao continua.

## Dependencias
- Nenhuma (task inicial da fase).

## Arquivos
- Create: `Mnemosyne.slnx`
- Create: `src/Mnemosyne.Api/Mnemosyne.Api.csproj`
- Create: `src/Mnemosyne.Application/Mnemosyne.Application.csproj`
- Create: `src/Mnemosyne.Domain/Mnemosyne.Domain.csproj`
- Create: `src/Mnemosyne.Infrastructure/Mnemosyne.Infrastructure.csproj`
- Create: `tests/Mnemosyne.UnitTests/Mnemosyne.UnitTests.csproj`
- Create: `tests/Mnemosyne.IntegrationTests/Mnemosyne.IntegrationTests.csproj`
- Create: `docs/implementation/learning-log.md`
- Create: `docs/implementation/error-fix-log.md`

## Planejamento (TDD)
1. RED: criar teste baseline de estrutura e executar para falhar (projetos ainda inexistentes).
2. GREEN: criar solucao/projetos e referencias minimas.
3. GREEN: adicionar projetos no `slnx` com `dotnet slnx add`.
4. VERIFY: executar testes unitarios baseline e validar sucesso.
5. REFACTOR: organizar pastas base (`Features`, `Entities`, `Persistence`).
6. DOCS: registrar aprendizado e erros/fixes iniciais nos logs.
7. COMMIT: `feat: inicializa solucao .NET 10 com arquivo slnx`.

## Comandos principais
```bash
dotnet new slnx -n Mnemosyne
dotnet new webapi -n Mnemosyne.Api -o src/Mnemosyne.Api -f net10.0 --use-minimal-apis
dotnet new classlib -n Mnemosyne.Application -o src/Mnemosyne.Application -f net10.0
dotnet new classlib -n Mnemosyne.Domain -o src/Mnemosyne.Domain -f net10.0
dotnet new classlib -n Mnemosyne.Infrastructure -o src/Mnemosyne.Infrastructure -f net10.0
dotnet new xunit -n Mnemosyne.UnitTests -o tests/Mnemosyne.UnitTests -f net10.0
dotnet new xunit -n Mnemosyne.IntegrationTests -o tests/Mnemosyne.IntegrationTests -f net10.0
dotnet test tests/Mnemosyne.UnitTests
```
