# Task 02 - CLI de memoria (remember/recall/list/show/delete)

## Objetivo
Implementar comandos de memoria no CLI consumindo os endpoints REST existentes.

## Dependencias
- Requer Task 01 da Fase 3 concluida.

## Arquivos
- Create: `cli/src/commands/memory/remember.ts`
- Create: `cli/src/commands/memory/recall.ts`
- Create: `cli/src/commands/memory/list.ts`
- Create: `cli/src/commands/memory/show.ts`
- Create: `cli/src/commands/memory/delete.ts`
- Modify: `cli/src/index.ts`
- Test: `cli/tests/memory/remember-command.test.ts`
- Test: `cli/tests/memory/recall-command.test.ts`
- Test: `cli/tests/memory/list-show-delete-command.test.ts`

## Planejamento (TDD)
1. RED: escrever testes de cada comando e parsing de options.
2. VERIFY RED: executar e confirmar falha por comandos inexistentes.
3. GREEN: implementar comandos e wiring no entrypoint do CLI.
4. VERIFY GREEN: executar testes e validar contrato de output.
5. REFACTOR: centralizar tratamento de erro e formatacao de saida.
6. DOCS: atualizar logs e ADR de formato de output (`table/json/yaml`).
7. COMMIT: `feat: adiciona comandos de memoria no cli`.

## Comandos principais
```bash
bun test cli/tests/memory/remember-command.test.ts
bun test cli/tests/memory/recall-command.test.ts
bun test cli/tests/memory/list-show-delete-command.test.ts
```
