# Task 06 - Deploy e observabilidade de producao

## Objetivo
Preparar empacotamento e monitoracao para ambiente de producao com runbooks operacionais.

## Dependencias
- Requer Task 03, Task 04 e Task 05 da Fase 4 concluidas.

## Arquivos
- Create: `deploy/docker-compose.prod.yml`
- Create: `deploy/k8s/api-deployment.yaml`
- Create: `deploy/k8s/api-service.yaml`
- Create: `deploy/k8s/api-hpa.yaml`
- Create: `docs/runbooks/operacao-producao.md`
- Create: `docs/runbooks/incidentes-comuns.md`
- Test: `deploy/tests/deploy-manifests.test.ts`

## Planejamento (TDD)
1. RED: escrever testes de validacao de manifests e configuracoes obrigatorias.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: criar artefatos de deploy e parametros de observabilidade.
4. VERIFY GREEN: executar validadores de manifests e smoke em ambiente controlado.
5. REFACTOR: padronizar variaveis e overlays de ambiente.
6. DOCS: publicar runbooks e atualizar logs de operacao.
7. COMMIT: `chore: prepara deploy e observabilidade de producao`.

## Comandos principais
```bash
bun test deploy/tests/deploy-manifests.test.ts
```
