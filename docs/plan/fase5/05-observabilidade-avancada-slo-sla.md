# Task 05 - Observabilidade avancada (SLO/SLA)

## Objetivo
Evoluir observabilidade com SLOs, alertas acionaveis e relatorios de SLA por periodo.

## Dependencias
- Requer Task 03 e Task 04 da Fase 5 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Infrastructure/Observability/SloCalculatorService.cs`
- Create: `src/Mnemosyne.Api/Endpoints/ObservabilityEndpoints.cs`
- Create: `dashboard/src/app/dashboard/sre/page.tsx`
- Create: `docs/sre/slo-definitions.md`
- Create: `docs/sre/alerting-playbook.md`
- Test: `tests/Mnemosyne.UnitTests/Infrastructure/Observability/SloCalculatorServiceTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Observability/SloEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de calculo de SLO e exposicao via endpoint.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar calculo de SLO, endpoints e pagina SRE no dashboard.
4. VERIFY GREEN: executar testes unitarios/integracao.
5. REFACTOR: separar regras por metrica (latencia, disponibilidade, erro).
6. DOCS: atualizar logs e ADR sobre metas operacionais.
7. COMMIT: `feat: adiciona observabilidade avancada com slo sla`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "SloCalculatorServiceTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "SloEndpointsTests"
```
