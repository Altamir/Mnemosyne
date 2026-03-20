# Task 03 - CLI de projeto (create/list/show/index/search)

## Objetivo
Implementar comandos de projetos e indexacao no CLI para operacao fim a fim sem dashboard.

## Dependencias
- Requer Task 02 da Fase 3 concluida.

## Arquivos
- Create: `cli/src/commands/project/create.ts`
- Create: `cli/src/commands/project/list.ts`
- Create: `cli/src/commands/project/show.ts`
- Create: `cli/src/commands/project/index.ts`
- Create: `cli/src/commands/project/search.ts`
- Modify: `cli/src/index.ts`
- Test: `cli/tests/project/project-commands.test.ts`

## Planejamento (TDD)
1. RED: escrever testes para comandos de projeto e seus argumentos.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar comandos e integracao com endpoints de projeto/index/search.
4. VERIFY GREEN: executar testes e validar resultados esperados.
5. REFACTOR: extrair validacoes comuns de `projectId/path/query`.
6. DOCS: atualizar logs continuos e ADR sobre UX de comandos de indexacao.
7. COMMIT: `feat: adiciona comandos de projeto e indexacao no cli`.

## Comandos principais
```bash
bun test cli/tests/project/project-commands.test.ts
```
