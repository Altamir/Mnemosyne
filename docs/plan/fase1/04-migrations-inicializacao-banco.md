# Task 04 - Migrations e inicializacao do banco

## Objetivo
Criar migration inicial e garantir bootstrap do banco na API.

## Dependencias
- Requer Task 03 concluida.

## Arquivos
- Create: `src/Mnemosyne.Infrastructure/Persistence/Migrations/*`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Persistence/MigrationTests.cs`

## Planejamento (TDD)
1. RED: escrever teste que valida existencia da migration inicial.
2. VERIFY RED: executar teste e confirmar falha.
3. GREEN: gerar migration (`InitialMemorySchema`) e configurar aplicacao no startup.
4. VERIFY GREEN: executar testes de migration e validar sucesso.
5. REFACTOR: extrair extensao de bootstrap de migrations para reduzir ruido no `Program.cs`.
6. DOCS: documentar fluxo de migrations e troubleshooting.
7. COMMIT: `feat: adiciona migration inicial e bootstrap de banco`.

## Comandos principais
```bash
dotnet ef migrations add InitialMemorySchema --project src/Mnemosyne.Infrastructure --startup-project src/Mnemosyne.Api
dotnet test tests/Mnemosyne.IntegrationTests --filter "MigrationTests"
```
