# Release Notes v1.0

## Funcionalidades Implementadas

### Core Features

1. **Criação de Memórias** (`POST /api/v1/memory/`)
   - Suporte para 4 tipos de memória: Note, Decision, Preference, Context
   - Validação de conteúdo (não pode ser vazio)
   - Persistência em PostgreSQL com pgvector

2. **Busca Semântica** (`GET /api/v1/memory/search`)
   - Busca por similaridade vetorial usando pgvector
   - Parâmetro `q` para query text
   - Parâmetro `topK` para quantidade de resultados

3. **Health Checks**
   - Liveness probe (`/health/live`)
   - Readiness probe (`/health/ready`) com verificação de conexão ao banco

### Infraestrutura

- Docker Compose com PostgreSQL (pgvector)
- Entity Framework Core com migrations
- Testcontainers para testes de integração
- xUnit com Moq e AutoFixture para testes

## Tech Stack

| Componente | Tecnologia |
|------------|------------|
| Runtime | .NET 10 |
| API | ASP.NET Core Minimal APIs |
| Database | PostgreSQL 18 + pgvector 0.8.2-pg18-trixie |
| ORM | Entity Framework Core 10 |
| Testing | xUnit, Moq, AutoFixture, Testcontainers |
| Architecture | Clean Architecture + CQRS |

## Limitações Conhecidas

1. **Sem autenticação** - API exposta sem autenticação (reservado para fase 2)
2. **Sem rate limiting** - endpoints sem proteção contra sobrecarga
3. **Embedding service mockado** - busca usa similaridade vetorial básica sem serviço de embedding dedicado
4. **Testes de integração requerem Docker** - não funcionam em ambientes sem container runtime

## Caminho para Produção

Para deploy em produção, considerar:

- [ ] Adicionar autenticação (API Key ou SSO)
- [ ] Implementar rate limiting
- [ ] Configurar monitoramento (Prometheus/Grafana)
- [ ] Adicionar logging estruturado
- [ ] Configurar backup automatizado do banco
- [ ] Implementar health checks mais robustos (memória, disco)
