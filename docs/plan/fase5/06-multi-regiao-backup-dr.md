# Task 06 - Multi-regiao, backup e disaster recovery

## Objetivo
Preparar arquitetura resiliente com estrategia multi-regiao, backup e recuperacao de desastre.

## Dependencias
- Requer Task 05 da Fase 5 concluida.

## Arquivos
- Create: `deploy/terraform/multiregion/main.tf`
- Create: `deploy/terraform/multiregion/variables.tf`
- Create: `deploy/terraform/multiregion/outputs.tf`
- Create: `docs/runbooks/disaster-recovery.md`
- Create: `docs/runbooks/backup-restore-procedure.md`
- Test: `deploy/tests/terraform-multiregion.test.ts`

## Planejamento (TDD)
1. RED: escrever testes de validacao de estrutura Terraform e requisitos minimos de DR.
2. VERIFY RED: executar e confirmar falha.
3. GREEN: implementar base Terraform e runbooks de DR/backup.
4. VERIFY GREEN: executar validacao de manifests e scripts de restore simulados.
5. REFACTOR: modularizar stacks por ambiente/regiao.
6. DOCS: atualizar logs e ADR de estrategia de continuidade de negocio.
7. COMMIT: `chore: adiciona base multiregiao com backup e dr`.

## Comandos principais
```bash
bun test deploy/tests/terraform-multiregion.test.ts
```
