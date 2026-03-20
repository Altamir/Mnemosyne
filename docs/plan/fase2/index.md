# Fase 2 - Index de tarefas

Este indice organiza as tasks da fase 2 em arquivos numerados, com ordem de implementacao e analise de paralelismo.

## Tarefas

1. `01-auth-api-key.md` - Autenticacao por API Key (middleware, handler e endpoint de validacao).
2. `02-projetos-crud.md` - CRUD de projetos com isolamento por usuario.
3. `03-indexacao-assincrona-projeto.md` - Indexacao assincrona por projeto com status.
4. `04-openai-embedding-service.md` - Servico de embeddings com OpenAI.
5. `05-compressao-contexto.md` - Compressao de contexto com estrategia default.
6. `06-grpc-search-index-compress.md` - Contratos e servicos gRPC para search/index/compress.
7. `07-observabilidade-healthchecks.md` - Observabilidade basica e health checks detalhados.
8. `08-hardening-fase2.md` - Hardening final, regressao e consolidacao documental.

## Sequencia recomendada de implementacao

- Ordem principal: 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 7 -> 8

### Justificativa de dependencias

- Task 1 estabelece autenticacao e contexto de seguranca para as APIs seguintes.
- Task 2 cria o contexto de projeto necessario para indexacao da Task 3.
- Task 4 habilita embeddings utilizados por index/search e compressao contextual.
- Task 5 depende da base de embeddings para estrategia semantica.
- Task 6 depende de index/search/compress ja implementados em camada de aplicacao.
- Task 7 depende dos endpoints/servicos existentes para verificar prontidao real.
- Task 8 fecha a fase com regressao e documentacao consolidada.

## Paralelismo validado

### Paralelizavel (com baixo risco)

- **Task 3 e Task 4**: podem ocorrer em paralelo apos Task 2, desde que mantenham contratos estaveis em `Application/Abstractions`.
- **Task 5 (parte de estrategia)** em paralelo com **Task 7 (estrutura de observabilidade)**, desde que endpoints de compressao nao sejam alterados no mesmo momento.
- **Documentacao continua** pode ser feita em paralelo por outro responsavel ao final de cada task tecnica.

### Nao paralelizavel (ou risco alto)

- Task 1 e Task 2: CRUD de projeto deve herdar regras de autenticacao.
- Task 6 com Task 3/4/5: gRPC depende dos casos de uso estaveis.
- Task 8 com qualquer task tecnica: hardening deve acontecer somente apos congelar features da fase.

## Observacoes de execucao

- Seguir TDD estrito em cada task: RED -> GREEN -> REFACTOR -> DOCS -> COMMIT.
- Codigo sempre em ingles.
- Documentacao sempre em pt-BR.

## Progresso

| # | Task | Status |
|---|------|--------|
| 01 | Autenticacao por API Key | Concluido |
| 02 | CRUD de projetos | Parcial (so Create, falta R/U/D + testes integracao) |
| 03 | Indexacao assincrona | Parcial (falta DI, ProjectIndexerService, testes integracao) |
| 04 | OpenAI Embedding Service | Parcial (falta DI, config, testes integracao) |
| 05 | Compressao de contexto | Parcial (falta testes integracao) |
| 06 | gRPC Services | Pendente |
| 07 | Observabilidade | Parcial (so endpoints manuais basicos) |
| 08 | Hardening | Pendente |
