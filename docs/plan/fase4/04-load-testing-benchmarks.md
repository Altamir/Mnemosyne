# Task 04 - Load testing e benchmarks

## Objetivo
Executar testes de carga para validar metas de latencia, throughput e estabilidade.

## Dependencias
- Requer Task 01 e Task 02 da Fase 4 concluidas.

## Arquivos
- Create: `perf/k6/api-latency.js`
- Create: `perf/k6/search-throughput.js`
- Create: `perf/k6/index-throughput.js`
- Create: `docs/perf/benchmark-report.md`
- Test: `tests/perf/perf-smoke.test.ts`

## Planejamento (TDD)
1. RED: escrever smoke test de perf para validar scripts e thresholds.
2. VERIFY RED: executar e confirmar falha inicial.
3. GREEN: implementar scripts de carga e thresholds alinhados ao PRD.
4. VERIFY GREEN: executar suite de perf e coletar resultados.
5. REFACTOR: ajustar cenarios e variaveis para repetibilidade.
6. DOCS: publicar benchmark report e atualizar logs de aprendizado/fixes.
7. COMMIT: `test: adiciona carga e benchmarks da plataforma`.

## Comandos principais
```bash
bun test tests/perf/perf-smoke.test.ts
k6 run perf/k6/api-latency.js
k6 run perf/k6/search-throughput.js
k6 run perf/k6/index-throughput.js
```
