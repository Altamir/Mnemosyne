# Task 06 - Dashboard (analytics, projetos e memorias)

## Objetivo
Implementar paginas de projetos, memorias e analytics com filtros e dados reais da API.

## Dependencias
- Requer Task 05 da Fase 3 concluida.

## Arquivos
- Create: `dashboard/src/app/dashboard/projects/page.tsx`
- Create: `dashboard/src/app/dashboard/memories/page.tsx`
- Create: `dashboard/src/app/dashboard/analytics/page.tsx`
- Create: `dashboard/src/components/projects-table.tsx`
- Create: `dashboard/src/components/memory-browser.tsx`
- Create: `dashboard/src/components/usage-charts.tsx`
- Test: `dashboard/tests/projects-page.test.tsx`
- Test: `dashboard/tests/memories-page.test.tsx`
- Test: `dashboard/tests/analytics-page.test.tsx`

## Planejamento (TDD)
1. RED: escrever testes de renderizacao, filtros e consumo de API por pagina.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar paginas e componentes de dados.
4. VERIFY GREEN: executar testes de interface e integracao com client.
5. REFACTOR: reduzir duplicacao de estados/loading/error.
6. DOCS: atualizar logs e ADR de estrategia de visualizacao de metricas.
7. COMMIT: `feat: adiciona paginas de analytics projetos e memorias no dashboard`.

## Comandos principais
```bash
bun test dashboard/tests/projects-page.test.tsx
bun test dashboard/tests/memories-page.test.tsx
bun test dashboard/tests/analytics-page.test.tsx
```
