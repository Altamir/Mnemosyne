# Mnemosyne

[![codecov](https://codecov.io/gh/Altamir/Mnemosyne/graph/badge.svg?token=7D5LM9GVAS)](https://codecov.io/gh/Altamir/Mnemosyne)

Sistema de memória semântica de código aberto para projetos de software. Mnemosyne permite que times de desenvolvimento armazenarem, pesquisarem e gerenciarem o contexto de projeto — decisões de design, preferências, histórico de decisões técnicas — usando embeddings vetoriais para busca semântica.

## Quick Start

### Pré-requisitos

- .NET 10 SDK
- Docker e Docker Compose
- Chave de API OpenAI (para geração de embeddings)

### 1. Subir dependências

```bash
docker-compose up -d
```

Isso inicia:
- **PostgreSQL** (porta 5432) com extensão pgvector

### 2. Configurar

Crie um arquivo `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MnemosyneDb": "Host=localhost;Database=mnemosyne;Username=postgres;Password=postgres"
  },
  "OpenAI": {
    "ApiKey": "sua-chave-openai-aqui",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

### 3. Executar a API

```bash
cd src/Mnemosyne.Api
dotnet run
```

A API estará disponível em `http://localhost:5226` (http) ou `https://localhost:7187` (https)

### 4. Executar testes

```bash
dotnet test
```

Para testes de integração com PostgreSQL real:
```bash
POSTGRES_USER=postgres POSTGRES_PASSWORD=postgres dotnet test
```

## API Endpoints

### Autenticação

Todos os endpoints (exceto `/api/v1/auth/*` e `/health/*`) requerem API Key no header `X-Api-Key`.

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/auth/validate` | Validar API Key |

### Projetos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/projects` | Criar projeto |
| GET | `/api/v1/projects` | Listar projetos do usuário |
| GET | `/api/v1/projects/{id}` | Obter projeto por ID |
| PUT | `/api/v1/projects/{id}` | Atualizar projeto |
| DELETE | `/api/v1/projects/{id}` | Deletar projeto |
| POST | `/api/v1/projects/{id}/index` | Iniciar indexação do projeto |
| GET | `/api/v1/projects/{id}/index/status` | Status da indexação |

### Memórias

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/memories` | Criar memória |
| GET | `/api/v1/memories/search?q=&topK=` | Buscar memórias semanticamente |

### Compressão

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/compress` | Comprimir código mantendo estrutura |

### Health Checks

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/health/live` | Liveness probe |
| GET | `/health/ready` | Readiness probe (verifica dependências) |
| GET | `/health` | Health check detalhado |

### gRPC

Serviços gRPC disponíveis na porta configurada:

- `SearchService` - Busca de memórias
- `IndexService` - Gerenciamento de indexação
- `CompressService` - Compressão de contexto

## Tech Stack

- **Runtime**: .NET 10
- **API**: ASP.NET Core Minimal APIs + gRPC
- **Database**: PostgreSQL 18 com pgvector 0.8.2
- **ORM**: Entity Framework Core
- **AI**: OpenAI API (embeddings)
- **Testing**: xUnit, Moq, AutoFixture, Testcontainers
- **Architecture**: Clean Architecture com CQRS

## Estrutura do Projeto

```
src/
├── Mnemosyne.Api/              # Camada de apresentação - Minimal APIs + gRPC
│   ├── Endpoints/              # Endpoints REST
│   ├── GrpcServices/          # Serviços gRPC
│   ├── Protos/                # Definições protobuf
│   └── Configuration/         # Health checks e configurações
├── Mnemosyne.Application/      # Casos de uso e handlers (CQRS)
├── Mnemosyne.Domain/           # Entidades, interfaces e regras de negócio
└── Mnemosyne.Infrastructure/   # Persistência, repositórios e serviços externos

tests/
├── Mnemosyne.UnitTests/        # Testes unitários
└── Mnemosyne.IntegrationTests/ # Testes de integração
```

## Funcionalidades Principais

### Fase 2 - Autenticação, Projetos e Infraestrutura

✅ **Autenticação por API Key**
- Hash BCrypt para armazenamento seguro
- Middleware de proteção de endpoints

✅ **Gerenciamento de Projetos**
- CRUD completo com isolamento multi-usuário
- Validação robusta de dados

✅ **Indexação Assíncrona**
- Jobs de indexação com filas
- Background service para processamento
- Status tracking em tempo real

✅ **Embeddings com OpenAI**
- Integração com OpenAI API
- Retry policy com exponential backoff
- Tratamento de erros transientes

✅ **Compressão de Contexto**
- Estratégia CodeStructure
- Preservação de assinaturas de métodos
- Remoção inteligente de código

✅ **Comunicação gRPC**
- Alta performance com comunicação binária
- 3 serviços: Search, Index, Compress

✅ **Observabilidade**
- Health checks detalhados
- Monitoramento de dependências (PostgreSQL, OpenAI)

## Documentação Adicional

- [Checklist Fase 2](docs/implementation/fase2-checklist.md)
- [Release Notes v0.2.0](docs/implementation/fase2-release-notes.md)
- [Learning Log](docs/implementation/learning-log.md)
- [ADR: Memory Domain Model](docs/implementation/adr/001-memory-domain-model.md)
