# Mnemosyne

[![codecov](https://codecov.io/gh/Altamir/Mnemosyne/graph/badge.svg?branch=main)](https://codecov.io/gh/Altamir/Mnemosyne)

Sistema de memória semântica de código aberto para projetos de software. Mnemosyne permite que times de desenvolvimento armazenarem, pesquisarem e gerenciarem o contexto de projeto — decisões de design, preferências, histórico de decisões técnicas — usando embeddings vetoriais para busca semântica.

## Quick Start

### Pré-requisitos

- .NET 10 SDK
- Docker e Docker Compose

### 1. Subir dependências

```bash
docker-compose up -d
```

Isso inicia:
- **PostgreSQL** (porta 5432) com extensão pgvector

### 2. Executar a API

```bash
cd src/Mnemosyne.Api
dotnet run
```

A API estará disponível em `http://localhost:5226` (http) ou `https://localhost:7187` (https)

### 3. Executar testes

```bash
dotnet test
```

Para testes de integração com PostgreSQL real:
```bash
POSTGRES_USER=postgres POSTGRES_PASSWORD=postgres dotnet test
```

## API Endpoints

### Memories

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/memory/` | Criar uma nova memória |
| GET | `/api/v1/memory/search?q=&topK=` | Buscar memórias por similaridade semântica |

### Health

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/health/live` | Liveness probe - retorna 200 se o serviço está rodando |
| GET | `/health/ready` | Readiness probe - retorna 200 se o banco de dados está acessível |

## Tech Stack

- **Runtime**: .NET 10
- **API**: ASP.NET Core Minimal APIs
- **Database**: PostgreSQL com pgvector (vetores de embeddings)
- **ORM**: Entity Framework Core
- **Testing**: xUnit, Moq, AutoFixture, Testcontainers
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API layers)

## Estrutura do Projeto

```
src/
├── Mnemosyne.Api/           # Camada de apresentação - Minimal APIs
├── Mnemosyne.Application/   # Casos de uso e handlers (CQRS)
├── Mnemosyne.Domain/        # Entidades e regras de negócio
└── Mnemosyne.Infrastructure/# Persistência e repositórios

tests/
├── Mnemosyne.UnitTests/         # Testes unitários
└── Mnemosyne.IntegrationTests/  # Testes de integração
```

## Documentação Adicional

- [ADR: Memory Domain Model](docs/implementation/adr/001-memory-domain-model.md)
- [Learning Log](docs/implementation/learning-log.md)
- [Error Fix Log](docs/implementation/error-fix-log.md)
