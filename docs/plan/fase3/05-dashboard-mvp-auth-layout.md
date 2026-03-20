# Task 05 - Dashboard MVP (auth e layout base)

## Objetivo
Criar base do Dashboard em Next.js com autenticacao, shell de navegacao e pagina overview inicial.

## Dependencias
- Requer Fase 2 concluida.

## Arquivos
- Create: `dashboard/package.json`
- Create: `dashboard/src/app/layout.tsx`
- Create: `dashboard/src/app/dashboard/page.tsx`
- Create: `dashboard/src/app/login/page.tsx`
- Create: `dashboard/src/lib/api-client.ts`
- Create: `dashboard/src/lib/auth.ts`
- Test: `dashboard/tests/auth-flow.test.tsx`
- Test: `dashboard/tests/dashboard-shell.test.tsx`

## Planejamento (TDD)
1. RED: escrever testes de fluxo de login e carregamento do shell autenticado.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar auth basica, layout e rota `/dashboard`.
4. VERIFY GREEN: executar testes de auth/layout.
5. REFACTOR: extrair componentes compartilhados de layout.
6. DOCS: atualizar logs e ADR da arquitetura do dashboard MVP.
7. COMMIT: `feat: adiciona dashboard mvp com autenticacao e shell`.

## Comandos principais
```bash
bun test dashboard/tests/auth-flow.test.tsx
bun test dashboard/tests/dashboard-shell.test.tsx
```
