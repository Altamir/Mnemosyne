# Docker Compose Setup

Configuração Docker completa para desenvolvimento local do Mnemosyne.

## Serviços

| Serviço | Descrição | Porta | Status |
|---------|-----------|-------|--------|
| `postgres` | PostgreSQL 18 + pgvector 0.8.2 | 5432 | Com health check |
| `api` | Aplicação .NET 10 (Mnemosyne.Api) | 5000/5001 | Com health check |

## Quick Start

```bash
# Copiar arquivo de exemplo
cp .env.example .env

# Adicionar sua chave da OpenAI no .env
# OPENAI_API_KEY=sua-chave-aqui

# Subir todos os serviços
docker compose up -d

# Verificar status
docker compose ps

# Logs
docker compose logs -f

# Parar
docker compose down
```

## Variáveis de Ambiente

Crie um arquivo `.env` baseado no `.env.example`:

```env
# PostgreSQL
POSTGRES_DB=mnemosyne
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# Application
ASPNETCORE_ENVIRONMENT=Development
OPENAI_API_KEY=sua-chave-aqui
```

## Endpoints

- **API HTTP**: http://localhost:5050
- **Health Check**: http://localhost:5050/health/live
- **PostgreSQL**: localhost:5432

## Comandos Úteis

```bash
# Rebuildar após mudanças
docker compose up -d --build

# Executar apenas o banco
docker compose up -d postgres

# Acessar o PostgreSQL
docker compose exec postgres psql -U postgres -d mnemosyne

# Limpar tudo (incluindo volumes)
docker compose down -v
```

## Dependências

O serviço `api` aguarda o PostgreSQL estar saudável antes de iniciar (`depends_on` com `condition: service_healthy`).
