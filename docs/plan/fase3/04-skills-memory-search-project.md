# Task 04 - Pacote de SKILLs (memory/search/project)

## Objetivo
Criar pacote inicial de SKILLs para integracao de agentes com o CLI do Mnemosyne.

## Dependencias
- Requer Task 01, Task 02 e Task 03 da Fase 3 concluidas.

## Arquivos
- Create: `skills/mnemosyne-memory/SKILL.md`
- Create: `skills/mnemosyne-search/SKILL.md`
- Create: `skills/mnemosyne-project/SKILL.md`
- Create: `skills/mnemosyne-memory/scripts/memory.ts`
- Create: `skills/mnemosyne-search/scripts/search.ts`
- Create: `skills/mnemosyne-project/scripts/project.ts`
- Test: `skills/tests/memory-skill.test.ts`
- Test: `skills/tests/search-skill.test.ts`
- Test: `skills/tests/project-skill.test.ts`

## Planejamento (TDD)
1. RED: escrever testes de comportamento esperado dos wrappers das SKILLs.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar scripts e `SKILL.md` para cada modulo.
4. VERIFY GREEN: executar testes e validar invocacao de comandos CLI.
5. REFACTOR: padronizar templates de `SKILL.md` e scripts compartilhados.
6. DOCS: atualizar logs e ADR da estrategia de distribuicao de SKILLs.
7. COMMIT: `feat: adiciona pacote inicial de skills mnemosyne`.

## Comandos principais
```bash
bun test skills/tests/memory-skill.test.ts
bun test skills/tests/search-skill.test.ts
bun test skills/tests/project-skill.test.ts
```
