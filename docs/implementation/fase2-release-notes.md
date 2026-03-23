# Release Notes - Mnemosyne v0.2.0

**Release Date:** 2026-03-22  
**Milestone:** Fase 2 - Autenticacao, Projetos e Infraestrutura  
**Status:** Concluido

## Resumo

Esta release adiciona funcionalidades essenciais para autenticacao, gerenciamento de projetos, indexacao assincrona, embeddings com OpenAI, compressao de contexto, comunicacao gRPC de alta performance e observabilidade completa.

## Novas Funcionalidades

### Autenticacao e Seguranca
- **API Key Authentication:** Sistema de autenticacao via header `X-Api-Key`
- **BCrypt Hashing:** Armazenamento seguro de chaves API
- **Middleware de Protecao:** Protecao automatica de endpoints

### Gerenciamento de Projetos
- **CRUD Completo:** Criar, listar, visualizar, atualizar e deletar projetos
- **Isolamento Multi-Usuario:** Projetos isolados por usuario
- **Validacao de Dados:** Validacao robusta em todos os endpoints

### Indexacao Assincrona
- **Jobs de Indexacao:** Sistema de filas para indexacao de projetos
- **Background Service:** Processamento assincrono com ProjectIndexerService
- **Status Tracking:** Acompanhamento em tempo real do status de indexacao
- **Prevencao de Duplicatas:** Protecao contra jobs duplicados

### Embeddings e IA
- **OpenAI Integration:** Geracao de embeddings via OpenAI API
- **Retry Policy:** Exponential backoff para resiliencia
- **Error Handling:** Tratamento de erros transientes (rate limits, etc.)
- **Vector Storage:** Armazenamento de vetores no PostgreSQL com pgvector

### Compressao de Contexto
- **Estrategia CodeStructure:** Remocao inteligente de codigo mantendo estrutura
- **Target Ratio Configuravel:** Controle de nivel de compressao
- **Multi-Strategy Support:** Arquitetura extensivel para novas estrategias

### Comunicacao gRPC
- **Alta Performance:** Comunicacao binaria eficiente
- **3 Servicos:** Search, Index e Compress
- **Streaming Support:** Base para comunicacao em tempo real

### Observabilidade
- **Health Checks Detalhados:** /health/live, /health/ready, /health
- **Dependencia Monitoring:** Checks para PostgreSQL e OpenAI
- **Structured Logging:** Base para logging estruturado

## Melhorias Tecnicas

### Arquitetura
- **Clean Architecture:** Separacao clara entre camadas
- **CQRS:** Commands e Queries separados
- **Repository Pattern:** Abstracao de acesso a dados
- **Dependency Injection:** Injecao de dependencias em toda aplicacao

### Qualidade de Codigo
- **Testes:** 151 testes (98 unitarios, 53 integracao)
- **Coverage:** 95-100% na camada Application
- **Code Style:** File-scoped namespaces, nullable enabled
- **TDD:** Desenvolvimento orientado a testes

### Performance
- **Compiled Regex:** [GeneratedRegex] para melhor performance
- **EF Core Optimization:** Queries eficientes com PostgreSQL
- **gRPC:** Comunicacao binaria rapida
- **Health Checks:** Verificacao rapida de dependencias

## Breaking Changes

Nao ha breaking changes nesta release. A API e retrocompativel com a Fase 1.

## Deprecacoes

Nenhuma deprecacao nesta release.

## Bugs Corrigidos

N/A - Esta e uma release de novas funcionalidades.

## Known Issues

1. **gRPC Search e Index:** Servicos implementados como placeholders, necessitam adaptacao dos handlers existentes
2. **OpenAI Coverage:** Baixa cobertura de testes devido a chamadas de API externa
3. **EF Core Warnings:** Warnings de versao devido a conflitos entre EF Core 10.0.4 e 10.0.5

## Migration Guide

Nao e necessario migration nesta release (novo projeto).

Para atualizar de versoes anteriores:
1. Executar migrations do EF Core
2. Configurar OpenAI:ApiKey no appsettings.json
3. Verificar health endpoints

## Documentacao

- [Checklist Fase 2](./fase2-checklist.md)
- [Learning Log](./learning-log.md)
- [API Documentation](./README.md)

## Agradecimentos

Agradecemos a todos que contribuiram para esta release atraves de feedback, testes e sugestoes.

## Links

- [Repositorio](https://github.com/Altamir/Mnemosyne)
- [Issues](https://github.com/Altamir/Mnemosyne/issues)
- [Documentacao](https://github.com/Altamir/Mnemosyne/tree/main/docs)

---

**Full Changelog:** [v0.1.0...v0.2.0](https://github.com/Altamir/Mnemosyne/compare/v0.1.0...v0.2.0)
