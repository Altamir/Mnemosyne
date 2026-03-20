# 🏛️ Mnemosyne

## Memory & Context Intelligence Platform

**Product Requirements Document (PRD) v1.0**

*March 2025*

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
   - [1.1 Problema](#11-problema)
   - [1.2 Solução](#12-solução)
   - [1.3 Proposta de Valor](#13-proposta-de-valor)
2. [Product Vision](#2-product-vision)
   - [2.1 Vision Statement](#21-vision-statement)
   - [2.2 Target Users](#22-target-users)
   - [2.3 Core Features](#23-core-features)
3. [System Architecture](#3-system-architecture)
   - [3.1 High-Level Architecture](#31-high-level-architecture)
   - [3.2 Component Overview](#32-component-overview)
   - [3.3 Data Model](#33-data-model)
4. [API Specification](#4-api-specification)
   - [4.1 Authentication](#41-authentication)
   - [4.2 REST Endpoints](#42-rest-endpoints)
   - [4.3 gRPC Services](#43-grpc-services)
5. [CLI Specification](#5-cli-specification)
   - [5.1 Installation](#51-installation)
   - [5.2 Configuration](#52-configuration)
   - [5.3 Commands](#53-commands)
6. [SKILL Specification](#6-skill-specification)
   - [6.1 Available SKILLs](#61-available-skills)
   - [6.2 SKILL Structure](#62-skill-structure)
   - [6.3 SKILL.md Example](#63-skillmd-example)
7. [Dashboard Specification](#7-dashboard-specification)
   - [7.1 Dashboard Features](#71-dashboard-features)
   - [7.2 Dashboard Pages](#72-dashboard-pages)
8. [Non-Functional Requirements](#8-non-functional-requirements)
   - [8.1 Performance](#81-performance)
   - [8.2 Scalability](#82-scalability)
   - [8.3 Security](#83-security)
9. [Implementation Roadmap](#9-implementation-roadmap)
10. [Success Metrics](#10-success-metrics)
11. [Appendix](#11-appendix)

---

## 1. Executive Summary

**Mnemosyne** (Μνημοσύνη) é uma plataforma de inteligência de memória e contexto projetada para assistentes de IA. Nomeada em homenagem à deusa grega da memória, mãe das nove musas, a plataforma permite que agentes de IA "lembrem" informações entre sessões, indexem codebases para busca semântica, e gerenciem contexto de forma eficiente com economia de até 60% no uso de tokens.

### 1.1 Problema

- Agentes de IA não têm memória persistente entre sessões
- Codebases grandes excedem limites de contexto (200K+ tokens)
- Custos elevados com tokens de contexto redundantes
- Falta de centralização para compartilhamento entre equipes

### 1.2 Solução

Uma plataforma distribuída que combina memória persistente, busca semântica em codebases, e compressão de contexto. Através de uma API escalável, CLI intuitivo, e SKILLs para integração direta com agentes, Mnemosyne reduz drasticamente o overhead de contexto enquanto mantém a inteligência do agente.

### 1.3 Proposta de Valor

| Métrica | Impacto |
|---------|---------|
| **Economia de Tokens** | 40-60% de redução em comparação com contexto tradicional |
| **Latência de Busca** | < 100ms para consultas semânticas |
| **Compressão de Contexto** | 70-98% de redução mantendo estrutura |
| **Memória Persistente** | Cross-session, cross-project, cross-team |

---

## 2. Product Vision

### 2.1 Vision Statement

> *"Tornar a memória e contexto dos agentes de IA tão naturais quanto a memória humana - persistentes, seletivas, e eficientes."*

### 2.2 Target Users

| Persona | Necessidades | Casos de Uso |
|---------|--------------|--------------|
| **Desenvolvedores** | Indexar projetos, buscar código, lembrar decisões | Code review, refactoring, debugging |
| **Equipes de IA** | Compartilhar contexto, métricas de uso | Agentes compartilhados, colaboração |
| **Empresas** | Segurança, auditoria, escalabilidade | Enterprise AI, compliance, multi-tenant |
| **Indivíduos** | Produtividade pessoal, automação | Personal AI assistant, knowledge mgmt |

### 2.3 Core Features

1. **Memory Store**: Armazenamento persistente de memórias com tipos (decision, note, preference, context)
2. **Semantic Index**: Indexação de codebases com embeddings vetoriais para busca semântica
3. **Context Compression**: Compressão inteligente de código mantendo estrutura (70-98% redução)
4. **Project Management**: Organização por projetos com isolamento e permissões
5. **Analytics Dashboard**: Visualização de uso, métricas de performance, custos

---

## 3. System Architecture

### 3.1 High-Level Architecture

A arquitetura do Mnemosyne segue um modelo distribuído em camadas, projetado para escalabilidade horizontal e baixa latência. O sistema é composto por quatro componentes principais que se comunicam através de protocolos otimizados para cada caso de uso.

```
┌──────────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                              │
│   ┌─────────┐   ┌─────────┐   ┌─────────┐   ┌─────────┐        │
│   │  Agent  │   │  SKILL  │   │   CLI   │   │Dashboard│        │
│   └────┬────┘   └────┬────┘   └────┬────┘   └────┬────┘        │
│        └─────────────┴──────┬──────┴─────────────┘              │
└─────────────────────────────┼───────────────────────────────────┘
                              │ REST/gRPC
┌─────────────────────────────┼───────────────────────────────────┐
│                      API GATEWAY                                 │
│   ┌──────────────────────────────────────────────────────┐     │
│   │  Auth (API Key)  │  Rate Limiting  │  Routing       │     │
│   └──────────────────────────────────────────────────────┘     │
└─────────────────────────────┬───────────────────────────────────┘
                              │
┌─────────────────────────────┼───────────────────────────────────┐
│                      SERVICE LAYER                               │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │  Memory  │ │  Index   │ │  Search  │ │Compress  │          │
│  │ Service  │ │ Service  │ │ Service  │ │ Service  │          │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘          │
└─────────────────────────────┬───────────────────────────────────┘
                              │
┌─────────────────────────────┼───────────────────────────────────┐
│                       DATA LAYER                                 │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐                       │
│  │PostgreSQL│ │  Redis   │ │   S3/    │                       │
│  │(pgvector)│ │ (Cache)  │ │MinIO     │                       │
│  └──────────┘ └──────────┘ └──────────┘                       │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Component Overview

| Componente | Tecnologia | Responsabilidade |
|------------|------------|------------------|
| **API Gateway** | Kong / Envoy | Auth, Rate Limit, Routing, SSL Termination |
| **REST API** | FastAPI / Bun + Hono | CRUD ops, config, validate, dashboard |
| **gRPC API** | gRPC + Protobuf | Index, search, compress (high performance) |
| **PostgreSQL** | PostgreSQL 16 + pgvector | Memories, projects, embeddings, FTS |
| **Redis** | Redis 7.x | Cache L1, rate limiting counters |
| **Object Storage** | S3 / MinIO | File storage, index snapshots |
| **CLI** | Bun + TypeScript | Client interface, config management |
| **Dashboard** | Next.js + shadcn/ui | Analytics, usage metrics, admin |

### 3.3 Data Model

O modelo de dados foi projetado para suportar multi-tenancy, busca vetorial eficiente, e consultas full-text nativas do PostgreSQL. A extensão pgvector permite armazenamento e busca de embeddings com alta performance.

| Entidade | Campos Principais | Descrição |
|----------|-------------------|-----------|
| **users** | id, email, api_key_hash, tier | Usuários com API keys e plano de serviço |
| **projects** | id, user_id, name, path, settings | Projetos indexados com configurações |
| **memories** | id, project_id, type, content, embedding | Memórias com embeddings para busca semântica |
| **code_chunks** | id, project_id, path, content, embedding | Fragmentos de código indexados com embeddings |
| **usage_logs** | id, user_id, endpoint, tokens, latency | Logs de uso para analytics e billing |

---

## 4. API Specification

### 4.1 Authentication

Todas as requisições à API requerem autenticação via API Key. A chave deve ser enviada no header `Authorization: Bearer <api_key>` para REST, ou como metadata no gRPC. A validação é feita contra um hash armazenado no banco de dados.

### 4.2 REST Endpoints

Os endpoints REST são otimizados para operações CRUD simples, configuração, e compatibilidade universal.

#### 4.2.1 Memory Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/memory` | Cria nova memória com embedding automático |
| GET | `/api/v1/memory/:id` | Recupera memória por ID |
| GET | `/api/v1/memory/search` | Busca semântica em memórias (query param) |
| PUT | `/api/v1/memory/:id` | Atualiza conteúdo da memória |
| DELETE | `/api/v1/memory/:id` | Remove memória |
| GET | `/api/v1/memory` | Lista memórias com filtros (type, project, date) |

#### 4.2.2 Project Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/project` | Cria novo projeto |
| GET | `/api/v1/project/:id` | Detalhes do projeto com estatísticas |
| GET | `/api/v1/project` | Lista projetos do usuário |
| PUT | `/api/v1/project/:id` | Atualiza configurações do projeto |
| DELETE | `/api/v1/project/:id` | Remove projeto e dados associados |
| POST | `/api/v1/project/:id/index` | Inicia indexação do projeto (async) |

#### 4.2.3 Auth & Config Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/v1/auth/validate` | Valida API Key e retorna info do usuário |
| POST | `/api/v1/auth/regenerate` | Regenera API Key (invalida anterior) |
| GET | `/api/v1/auth/usage` | Retorna uso atual e limites do plano |

### 4.3 gRPC Services

Os serviços gRPC são otimizados para operações de alta performance: indexação, busca semântica, e compressão de contexto. Utilizam streaming para operações longas e payloads binários Protobuf para menor overhead.

| Service | RPC Methods | Use Case |
|---------|-------------|----------|
| **IndexService** | IndexProject (stream), GetIndexStatus | Indexação progressiva com feedback |
| **SearchService** | SemanticSearch, KeywordSearch, HybridSearch | Busca de alta performance |
| **CompressService** | CompressCode, CompressContext | Compressão com streaming de resultado |

---

## 5. CLI Specification

### 5.1 Installation

O CLI do Mnemosyne é distribuído como pacote npm/bun e pode ser instalado globalmente.

```bash
# Via npm
npm install -g @mnemosyne/cli

# Via bun
bun install -g @mnemosyne/cli
```

### 5.2 Configuration

O CLI suporta configuração via variáveis de ambiente ou arquivo `~/.mnemosyne/config.json`. As variáveis de ambiente têm precedência.

| Env Variable | Config Key | Descrição |
|--------------|------------|-----------|
| `MNEMOSYNE_API_URL` | apiUrl | URL base da API (default: https://api.mnemosyne.io) |
| `MNEMOSYNE_API_KEY` | apiKey | API Key para autenticação |
| `MNEMOSYNE_OUTPUT` | output | Formato de saída: json, yaml, table (default: table) |

### 5.3 Commands

#### 5.3.1 Config Command

```bash
mnemosyne config [options]
```

Gerencia a configuração do CLI.

| Option | Descrição |
|--------|-----------|
| `--url <url>` | Define a URL da API |
| `--key <key>` | Define a API Key |
| `--show` | Mostra configuração atual (mascara API Key) |
| `--reset` | Remove configuração salva |

#### 5.3.2 Validate Command

```bash
mnemosyne validate
```

Valida se a API Key configurada está ativa e retorna informações do usuário e limites do plano.

**Output example:**
```
✓ API Key valid
  User: user@example.com
  Plan: Pro
  Usage: 15,420 / 100,000 tokens
```

#### 5.3.3 Memory Commands

| Command | Descrição |
|---------|-----------|
| `mnemosyne remember <content>` | Armazena nova memória |
| `mnemosyne recall <query>` | Busca memórias por similaridade semântica |
| `mnemosyne memories list` | Lista todas as memórias com filtros |
| `mnemosyne memories show <id>` | Mostra detalhes de uma memória |
| `mnemosyne memories delete <id>` | Remove uma memória |

**Options for remember:**
- `--type <type>` - Tipo: decision, note, preference, context (default: note)
- `--project <id>` - Associa a um projeto específico
- `--tags <tags>` - Tags separadas por vírgula

#### 5.3.4 Project Commands

| Command | Descrição |
|---------|-----------|
| `mnemosyne project create <name>` | Cria novo projeto |
| `mnemosyne project list` | Lista projetos do usuário |
| `mnemosyne project show <id>` | Mostra detalhes e estatísticas |
| `mnemosyne project index <path>` | Indexa diretório para o projeto |
| `mnemosyne project search <query>` | Busca semântica no projeto |

#### 5.3.5 Compress Command

```bash
mnemosyne compress <file|stdin>
```

Comprime código mantendo estrutura, removendo detalhes. Redução de 70-98% mantendo compreensão.

**Options:**
- `--strategy <s>` - code_structure, signatures, summary (default: code_structure)
- `--output <file>` - Arquivo de saída (default: stdout)

---

## 6. SKILL Specification

As SKILLs do Mnemosyne permitem integração direta com AI Agents, carregando funcionalidade sob demanda e economizando tokens de contexto.

### 6.1 Available SKILLs

| SKILL | Operações | Descrição |
|-------|-----------|-----------|
| **mnemosyne-memory** | remember, recall | Gerenciamento de memórias persistentes |
| **mnemosyne-search** | search, index, compress | Busca semântica e compressão de contexto |
| **mnemosyne-project** | project info, list, stats | Gerenciamento de projetos |

### 6.2 SKILL Structure

Cada SKILL segue o padrão do ecossistema:

```
skills/
├── mnemosyne-memory/
│   ├── SKILL.md          # Documentação e instruções
│   ├── scripts/
│   │   └── memory.ts     # Implementação do CLI wrapper
│   └── LICENSE.txt
```

### 6.3 SKILL.md Example

```markdown
# Mnemosyne Memory SKILL

Persistent memory operations for AI agents.

## Prerequisites

- MNEMOSYNE_API_KEY environment variable
- CLI installed: `bun install -g @mnemosyne/cli`

## Available Operations

### remember
Store a new memory with automatic embedding.

### recall
Search memories by semantic similarity.

## Usage

The CLI will be invoked automatically when the skill is loaded.
```

---

## 7. Dashboard Specification

O Dashboard do Mnemosyne fornece visualização em tempo real de uso, métricas de performance, e ferramentas de administração. Construído com Next.js e shadcn/ui para uma experiência moderna e responsiva.

### 7.1 Dashboard Features

| Feature | Descrição |
|---------|-----------|
| **Usage Overview** | Gráficos de uso de tokens, requisições, latência média por período |
| **Project Stats** | Estatísticas por projeto: chunks indexados, memórias, buscas |
| **Memory Browser** | Interface visual para navegar e buscar memórias |
| **API Key Management** | Gerar, regenerar, revogar API Keys |
| **Cost Calculator** | Estimativa de custos baseada em uso real |
| **Audit Log** | Histórico de operações com filtros e export |

### 7.2 Dashboard Pages

1. `/dashboard` - Overview com KPIs principais e gráficos de tendência
2. `/dashboard/projects` - Lista e detalhes de projetos
3. `/dashboard/memories` - Browser de memórias com busca
4. `/dashboard/analytics` - Relatórios detalhados e export
5. `/dashboard/settings` - Configurações de conta e API Keys

---

## 8. Non-Functional Requirements

### 8.1 Performance

| Métrica | Target | Nota |
|---------|--------|------|
| API Response Time (p95) | < 100ms | Exceto indexação |
| Search Latency (p95) | < 50ms | Busca semântica |
| Index Throughput | > 1000 files/min | Por instância |
| CLI Startup | < 50ms | Cold start |

### 8.2 Scalability

- Suportar 10,000+ usuários concorrentes
- 100+ projetos por usuário
- 1M+ memórias por projeto
- 10M+ code chunks indexados

### 8.3 Security

1. API Keys hasheadas com bcrypt
2. TLS 1.3 para todas as comunicações
3. Rate limiting por API Key
4. Audit log de todas as operações
5. Isolamento multi-tenant

---

## 9. Implementation Roadmap

### Phase 1: Foundation (Weeks 1-4)

| Task | Descrição | Entregável |
|------|-----------|------------|
| Database Schema | PostgreSQL + pgvector setup, migrations | Schema v1 |
| Core API | REST endpoints: auth, memory, project | API v1 |
| Auth System | API Key generation, validation, hashing | Auth module |
| CLI Core | CLI structure, config, validate commands | CLI alpha |

### Phase 2: Core Features (Weeks 5-8)

| Task | Descrição | Entregável |
|------|-----------|------------|
| Semantic Search | Embeddings, vector search, hybrid search | Search API |
| Code Indexing | File parsing, chunking, embedding pipeline | Index service |
| Context Compression | Code structure extraction, compression rules | Compress API |
| CLI Full | All CLI commands, output formats | CLI v1.0 |

### Phase 3: Integration (Weeks 9-12)

| Task | Descrição | Entregável |
|------|-----------|------------|
| gRPC API | Protobuf definitions, gRPC server | gRPC v1 |
| SKILLs | memory, search, project SKILLs | SKILLs pkg |
| Dashboard MVP | Next.js app, auth, basic analytics | Dashboard v1 |
| Documentation | API docs, CLI docs, SKILL guides | Docs site |

### Phase 4: Scale & Polish (Weeks 13-16)

| Task | Descrição | Entregável |
|------|-----------|------------|
| Redis Cache | L1 cache, rate limiting counters | Cache layer |
| Dashboard Full | All pages, real-time metrics, export | Dashboard v2 |
| Load Testing | Performance optimization, benchmarks | Perf report |
| Beta Launch | Public beta, feedback collection | Public beta |

---

## 10. Success Metrics

Os seguintes KPIs serão usados para medir o sucesso do Mnemosyne após o lançamento do beta público:

| KPI | Target (Mês 3) | Target (Mês 6) |
|-----|----------------|----------------|
| Usuários Ativos Mensais | 500 | 2,000 |
| Projetos Indexados | 1,000 | 5,000 |
| Memórias Armazenadas | 50,000 | 250,000 |
| Token Savings Rate | > 40% | > 50% |
| API Uptime | > 99.5% | > 99.9% |
| NPS Score | > 30 | > 50 |

---

## 11. Appendix

### 11.1 Glossary

| Termo | Definição |
|-------|-----------|
| **Embedding** | Representação vetorial de texto que captura significado semântico |
| **Vector Search** | Busca por similaridade usando distância vetorial (cosine, euclidean) |
| **Context Window** | Limite de tokens que um modelo de IA pode processar |
| **Chunk** | Fragmento de código ou texto extraído para indexação |
| **SKILL** | Módulo de funcionalidade carregado sob demanda por AI Agents |

### 11.2 References

1. pgvector Extension: https://github.com/pgvector/pgvector
2. gRPC Documentation: https://grpc.io/docs/
3. OpenAPI Specification: https://spec.openapis.org/oas/latest.html
4. shadcn/ui: https://ui.shadcn.com/

### 11.3 Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | March 2025 | Product Team | Initial PRD |

---

*Document generated for Mnemosyne Project - Memory & Context Intelligence Platform*
