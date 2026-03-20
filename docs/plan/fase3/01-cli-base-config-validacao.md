# Task 01 - CLI base (config e validacao)

## Objetivo
Criar o CLI inicial do Mnemosyne com comandos de configuracao e validacao de API Key.

## Dependencias
- Requer Fase 2 concluida.

## Arquivos
- Create: `cli/package.json`
- Create: `cli/src/index.ts`
- Create: `cli/src/commands/config.ts`
- Create: `cli/src/commands/validate.ts`
- Create: `cli/src/lib/client.ts`
- Create: `cli/src/lib/config-store.ts`
- Test: `cli/tests/config-command.test.ts`
- Test: `cli/tests/validate-command.test.ts`

## Planejamento (TDD)
1. RED: escrever testes para `mnemosyne config --url/--key/--show` e `mnemosyne validate`.
2. VERIFY RED: executar testes do CLI e confirmar falha inicial.
3. GREEN: implementar comandos minimos e cliente HTTP para `/api/v1/auth/validate`.
4. VERIFY GREEN: executar testes unitarios do CLI.
5. REFACTOR: extrair parse de argumentos e persistencia de config para modulos dedicados.
6. DOCS: atualizar logs continuos e criar ADR da estrategia de armazenamento de config local.
7. COMMIT: `feat: adiciona cli base com config e validacao`.

## Comandos principais
```bash
bun test cli/tests/config-command.test.ts
bun test cli/tests/validate-command.test.ts
```
