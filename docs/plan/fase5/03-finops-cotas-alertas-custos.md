# Task 03 - FinOps (cotas, alertas e custos)

## Objetivo
Implementar controles financeiros por tenant: cotas, alertas proativos e visao de custo detalhada.

## Dependencias
- Requer Task 02 da Fase 5 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/BillingPlanEntity.cs`
- Create: `src/Mnemosyne.Domain/Entities/UsageQuotaEntity.cs`
- Create: `src/Mnemosyne.Application/Features/Billing/CheckQuota/CheckQuotaQuery.cs`
- Create: `src/Mnemosyne.Application/Features/Billing/GenerateCostReport/GenerateCostReportQuery.cs`
- Create: `src/Mnemosyne.Api/Endpoints/BillingEndpoints.cs`
- Create: `dashboard/src/app/dashboard/finops/page.tsx`
- Test: `tests/Mnemosyne.UnitTests/Application/Billing/QuotaAndCostTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Api/Billing/BillingEndpointsTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para bloqueio por quota e calculo de custos.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar entidades, queries e endpoints de billing/finops.
4. VERIFY GREEN: executar testes backend + validacoes de endpoint.
5. REFACTOR: separar regras de precificacao por provider/plano.
6. DOCS: atualizar logs e ADR de politica de quotas e alertas.
7. COMMIT: `feat: adiciona finops com cotas alertas e custos`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "QuotaAndCostTests"
dotnet test tests/Mnemosyne.IntegrationTests --filter "BillingEndpointsTests"
```
