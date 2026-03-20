# Fase 3 - Index de tarefas

Este indice organiza as tasks da fase 3 em arquivos numerados, com ordem de implementacao e validacao de paralelismo.

## Tarefas

1. `01-cli-base-config-validacao.md` - CLI base com configuracao e validacao de API Key.
2. `02-cli-memoria-comandos.md` - Comandos de memoria no CLI.
3. `03-cli-projeto-index-search.md` - Comandos de projeto/index/search no CLI.
4. `04-skills-memory-search-project.md` - Pacote inicial de SKILLs integrando com CLI.
5. `05-dashboard-mvp-auth-layout.md` - Dashboard MVP com auth e layout base.
6. `06-dashboard-analytics-projetos-memorias.md` - Paginas de analytics/projetos/memorias.
7. `07-documentacao-oficial-api-cli-skills.md` - Documentacao oficial da fase.
8. `08-hardening-release-fase3.md` - Hardening final e release da fase 3.

## Sequencia recomendada de implementacao

- Ordem principal: 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 7 -> 8

### Justificativa de dependencias

- Tasks 1, 2 e 3 constroem o eixo CLI progressivamente.
- Task 4 depende do CLI funcional para os wrappers de SKILL.
- Tasks 5 e 6 constroem o dashboard em duas camadas (base e paginas).
- Task 7 depende de CLI, SKILLs e dashboard estarem estaveis.
- Task 8 finaliza com regressao e fechamento documental.

## Paralelismo validado

### Paralelizavel (com baixo risco)

- **Trilha CLI e trilha Dashboard** podem rodar em paralelo apos Fase 2:
  - Trilha A: Tasks 1 -> 2 -> 3 -> 4
  - Trilha B: Tasks 5 -> 6
- **Task 7** pode iniciar parcialmente em paralelo ao fim da Task 4 e Task 6 (esqueleto da documentacao), finalizando apenas apos ambas.

### Nao paralelizavel (ou risco alto)

- Task 2 antes da Task 1 (dependencia direta de base CLI).
- Task 4 antes da Task 3 (SKILL wrappers dependem dos comandos).
- Task 8 em paralelo com implementacao funcional (risco de fechar regressao prematuramente).

## Observacoes de execucao

- Seguir TDD estrito em cada task: RED -> GREEN -> REFACTOR -> DOCS -> COMMIT.
- Codigo sempre em ingles.
- Documentacao sempre em pt-BR.
