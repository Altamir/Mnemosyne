# AGENTS.md

## Visão Geral do Projeto

Mnemosyne é um sistema de gerenciamento de memórias semântico com indexação vetorial.

## Stack Tecnológica

- **.NET 10** com C#
- **PostgreSQL + pgvector** para persistência vetorial
- **xUnit + Moq + AutoFixture** para testes
- **Minimal APIs** com endpoints registradores

## Estrutura do Projeto

```
src/
├── Mnemosyne.Api/              # Endpoints, Middleware, Program.cs
├── Mnemosyne.Application/      # Handlers, Commands, Queries
├── Mnemosyne.Domain/           # Entities, Interfaces, Enums
├── Mnemosyne.Infrastructure/    # Repositories, DbContext, Configurations
tests/
├── Mnemosyne.UnitTests/        # Testes unitários
└── Mnemosyne.IntegrationTests/  # Testes de integração (PostgreSQL)
```

## Convenções Obrigatórias

### Código
- **Código em inglês** (classes, métodos, variáveis)
- **Documentação em pt-BR** (XML, README, .md)
- **Commits em pt-BR** (Conventional Commits)
- **Mensagens de erro em pt-BR** para o usuário

### Nomenclatura
| Tipo | Padrão |
|------|--------|
| Command | `CreateMemoryCommand`, `DeleteProjectCommand` |
| Query | `SearchMemoryQuery`, `GetProjectByIdQuery` |
| Handler | `CreateMemoryHandler`, `SearchMemoryHandler` |
| Repository | `IMemoryRepository`, `MemoryRepository` |
| Endpoint | `MemoryEndpoints`, `ProjectEndpoints` |

### Camadas
- `Domain/` - Entidades, Interfaces de Repositório, Enums
- `Application/Features/` - Commands, Queries, Handlers
- `Infrastructure/Repositories/` - Implementações de Repositório
- `Api/Endpoints/` - Minimal API endpoints

## Skills Obrigatórias

Ao trabalhar neste projeto, **SEMPRE** carregue estas skills:

```
@using-superpowers     # Obrigatório em qualquer conversa
@dotnet-conventions    # Ao escrever código .NET
@xunit-tests          # Ao criar/modificar testes
@commit-safety        # Ao fazer commits
@test-driven-development # Ao implementar funcionalidades
```

## TDD Workflow

```
RED  -> Escrever teste primeiro, assistir falha
GREEN -> Implementar código mínimo para passar
REFACTOR -> Limpar código mantendo testes verdes
```

### Ciclo de Desenvolvimento

1. Escrever teste (RED)
2. Executar `dotnet test` - confirmar falha
3. Implementar código mínimo (GREEN)
4. Executar `dotnet test` - confirmar sucesso
5. Refatorar se necessário
6. Commitar

## Comandos Úteis

```bash
# Compilar
dotnet build

# Testes unitários
dotnet test tests/Mnemosyne.UnitTests

# Testes de integração
dotnet test tests/Mnemosyne.IntegrationTests

# Todos os testes
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório de cobertura
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coverage"
```

## Commits

### Formato
```
<tipo>(<escopo>): <descrição>

<corpo opcional>
```

### Tipos
- `feat` - Nova funcionalidade
- `fix` - Correção de bug
- `docs` - Documentação
- `test` - Testes
- `chore` - Tarefas diversas (deps, config)
- `refactor` - Refatoração

### Exemplo
```bash
git commit -m "feat: adiciona endpoint de busca semântica

- adiciona SearchMemoryQuery e handler
- integra com pgvector para busca por similaridade
- adiciona testes unitários"
```

## Commits Atômicos

**Regra:** Um commit por tarefa lógica. Nunca misturar:
- Mudanças de código com mudanças de documentação
- Múltiplas features no mesmo commit
- Fixes não relacionados no mesmo commit

## Desenvolvimento de Features

### Plano de Implementação
Quando implementar features complexas, criar plano em `docs/plans/` seguindo skill `@writing-plans`.

### Tasks da Fase 2 (Exemplo)
1. Task 01: Autenticacao API Key
2. Task 02: CRUD de Projetos
3. Task 03: Indexacao Assíncrona
4. Task 04: OpenAI Embedding Service
5. Task 05: Compressao de Contexto
6. Task 06: gRPC Services
7. Task 07: Observabilidade
8. Task 08: Hardening

## Pastas a Ignorar

```
bin/
obj/
Debug/
Release/
coverage/
*.user
*.suo
```

## Recursos

- [dotnet-conventions skill](./.config/opencode/skills/dotnet-conventions/SKILL.md)
- [xunit-tests skill](./.config/opencode/skills/xunit-tests/SKILL.md)
- [test-driven-development skill](./.config/opencode/skills/superpowers/test-driven-development/SKILL.md)
- [commit-safety skill](./.config/opencode/skills/commit-safety/SKILL.md)
