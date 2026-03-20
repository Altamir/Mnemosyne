# Task 07 - Documentacao oficial (API, CLI e SKILLs)

## Objetivo
Publicar documentacao oficial da fase 3 cobrindo API, CLI, SKILLs e dashboard MVP.

## Dependencias
- Requer Task 04 e Task 06 da Fase 3 concluidas.

## Arquivos
- Create: `docs/reference/api.md`
- Create: `docs/reference/cli.md`
- Create: `docs/reference/skills.md`
- Create: `docs/reference/dashboard.md`
- Modify: `README.md`
- Test: `docs/tests/documentation-links.test.ts`

## Planejamento (TDD)
1. RED: escrever teste de validacao de links e secoes obrigatorias da doc.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: escrever documentacao completa em pt-BR.
4. VERIFY GREEN: executar teste de links e validacao de estrutura.
5. REFACTOR: padronizar formato e remover duplicacoes entre docs.
6. DOCS: atualizar learning/error-fix logs com pontos de usabilidade documental.
7. COMMIT: `docs: publica documentacao oficial da fase 3`.

## Comandos principais
```bash
bun test docs/tests/documentation-links.test.ts
```
