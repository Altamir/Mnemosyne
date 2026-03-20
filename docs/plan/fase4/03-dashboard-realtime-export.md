# Task 03 - Dashboard full (tempo real e export)

## Objetivo
Evoluir dashboard para visualizacao em tempo real e exportacao de relatorios.

## Dependencias
- Requer Task 02 da Fase 4 concluida.

## Arquivos
- Create: `dashboard/src/app/dashboard/realtime/page.tsx`
- Create: `dashboard/src/app/dashboard/export/page.tsx`
- Create: `dashboard/src/components/realtime-kpi-cards.tsx`
- Create: `dashboard/src/components/export-report-form.tsx`
- Create: `dashboard/src/lib/realtime-client.ts`
- Test: `dashboard/tests/realtime-page.test.tsx`
- Test: `dashboard/tests/export-page.test.tsx`

## Planejamento (TDD)
1. RED: escrever testes de feed em tempo real e geracao de export.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar paginas/componentes e integracao de dados.
4. VERIFY GREEN: executar testes de UI e integracao de client.
5. REFACTOR: reduzir duplicacao de polling/subscription e estado de filtros.
6. DOCS: atualizar logs e ADR de estrategia de atualizacao em tempo real.
7. COMMIT: `feat: adiciona dashboard realtime e exportacao`.

## Comandos principais
```bash
bun test dashboard/tests/realtime-page.test.tsx
bun test dashboard/tests/export-page.test.tsx
```
