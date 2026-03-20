# Learning Log

## Fase 2 - Autenticacao e Projetos

### 2026-03-20

#### Task 05 - Compressao de Contexto
- Strategy pattern via DI com IEnumerable<ICompressionStrategy> — sem factory class
- Handler resolve estrategia por StrategyName comparado com enum.ToString()
- CodeStructureCompressionStrategy usa [GeneratedRegex] para performance e AOT compatibility
- Compressao remove: corpos de metodo, comentarios (// /// /* */), linhas em branco excessivas
- Compressao preserva: declaracoes de classe/interface/enum/record, assinaturas de metodo, auto-properties, usings, namespaces
- CompressionResult record retorna metadata: compressed content, original/compressed length, ratio, strategy name
- targetRatio validado entre 0 (exclusivo) e 1 (exclusivo), default 0.7
- Estrategias registradas como Singleton (stateless), handler como Scoped
- 100% line coverage e 98% branch coverage nos arquivos de compressao
- 21 testes unitarios (9 handler + 12 strategy) cobrindo todos os cenarios

#### Task 04 - OpenAI Embedding Service
- Implementado IEmbeddingService interface em Domain.Interfaces
- OpenAiEmbeddingService usa EmbeddingClient do OpenAI SDK 2.9.1
- CreateMemoryHandler agora injeta IEmbeddingService e gera embedding ao criar memoria
- Metodo SetEmbedding adicionado a MemoryEntity para definir embedding pos-criacao
- Modelo text-embedding-3-large usado por padrao
- Interface IEmbeddingService permite mock em testes unitarios

#### Task 03 - Indexacao Assincrona de Projeto
- Implementado ProjectIndexJobEntity para rastrear Jobs de indexacao
- IndexStatus enum: Pending, Processing, Completed, Failed
- StartProjectIndexCommand/Handler inicia job de indexacao
- GetIndexStatusQuery/Handler retorna status do job mais recente
- Protecao contra duplicate index jobs (InvalidOperationException se ja em andamento)
- Endpoint POST /api/v1/projects/{id}/index para iniciar indexacao
- Endpoint GET /api/v1/projects/{id}/index/status para consultar status
- ProjectIndexJobRepository com GetPendingOrProcessingByProjectIdAsync
- Configuration EntityFramework para project_index_jobs table

#### Task 01 - Autenticacao API Key
- Implementado UserEntity com hash BCrypt para seguranca de API Keys
- API Key传出 via header `X-Api-Key`
- ValidateApiKeyHandler como Query Handler para validacao
- ApiKeyMiddleware protege todos os endpoints exceto `/api/v1/auth/*`
- Middleware injeta UserId no HttpContext.Items para acesso posterior

#### Task 02 - CRUD de Projetos
- ProjectEntity com isolamento logico por UserId
- Repository pattern com IProjectRepository
- Endpoint POST /api/v1/projects com CreateProjectHandler
- Configuracao EF Core para Projects com indice em UserId

## Fase 1 - Foundation

### 2026-03-19

#### Bootstrap Setup
- Created .NET 10 solution with 6 projects
- Note: `dotnet new sln` creates `.slnx` file on this system, not `.sln`
- Project structure: Api, Application, Domain, Infrastructure + UnitTests, IntegrationTests
- Added project references following clean architecture layers

#### Open Questions
- Need to verify if .slnx extension causes any issues with tooling

### Memory Domain Model (Task 02)
- Implemented MemoryEntity and MemoryType using TDD approach
- Created minimal domain model with guard clause for empty content
- Used factory pattern (Create method) for object creation
- Null-forgiving operator (`= null!`) used for private setter initialization
