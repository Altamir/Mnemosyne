# Fase 1 - Index de tarefas

Este indice organiza as tasks em arquivos separados, numerados e com ordem de implementacao.

## Tarefas

1. `01-bootstrap-slnx.md` - Bootstrap da solucao com `.slnx`, projetos e estrutura inicial.
2. `02-modelo-dominio-memoria.md` - Modelo de dominio de memoria (`MemoryEntity` e `MemoryType`).
3. `03-persistencia-postgresql-pgvector.md` - Persistencia com PostgreSQL 17.9 + pgvector 0.8.2.
4. `04-migrations-inicializacao-banco.md` - Migrations e inicializacao automatica de banco.
5. `05-create-memory.md` - Fluxo CreateMemory (command/handler/endpoint).
6. `06-search-memory-pgvector.md` - Fluxo SearchMemory com busca vetorial.
7. `07-e2e-integracao.md` - Testes E2E com Testcontainers e fixture de PostgreSQL.
8. `08-fechamento-qualidade-documentacao.md` - Fechamento de qualidade, health checks e docs de release.

## Sequencia recomendada de implementacao

- Ordem principal: 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 7 -> 8

### Justificativa de dependencias

- Task 1 e pre-requisito estrutural para todas as demais.
- Task 2 define o modelo que sera persistido na Task 3.
- Task 4 depende da Task 3 (nao ha migration sem mapeamento pronto).
- Task 5 precisa do dominio e da base de persistencia/migration.
- Task 6 depende de CreateMemory + persistencia vetorial pronta.
- Task 7 depende dos fluxos create/search concluidos.
- Task 8 consolida tudo e deve ser a ultima.

## Paralelismo validado

### Paralelizavel

- **Task 2 + preparacao de docs base (parcial da Task 8)**: possivel executar em paralelo apenas para escrita de templates de documentacao (sem alterar comportamento da API).
- **Dentro da Task 7**: parte de `docker-compose.yml` e parte da fixture de teste podem ser desenvolvidas em paralelo por pessoas diferentes, com merge no final.

### Nao paralelizavel (ou risco alto)

- Task 3 e Task 4: acopladas por schema/migration.
- Task 5 e Task 6: search depende do fluxo de create e repositorio estabilizado.
- Task 6 e Task 7: E2E depende dos endpoints funcionando.

## Observacoes de execucao

- Seguir TDD estrito em cada task: RED -> GREEN -> REFACTOR -> DOCS -> COMMIT.
- Codigo sempre em ingles.
- Documentacao sempre em pt-BR.
