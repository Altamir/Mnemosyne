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
- DI registration no Program.cs: OpenAIClient, EmbeddingClient, IEmbeddingService
- Configuracao OpenAI:ApiKey e OpenAI:EmbeddingModel no appsettings.json
- Retry policy com exponential backoff (MaxRetries=3, InitialDelay=1000ms)
- Tratamento de erros transientes: 429 rate limit, 5xx server errors, network errors
- Testes de integracao verificam DI registration e resolucao de servicos
- 3 testes unitarios para validacao de input (empty, whitespace, null)
- 5 testes de integracao para DI registration e validacao

#### Task 03 - Indexacao Assincrona de Projeto
- Implementado ProjectIndexJobEntity para rastrear Jobs de indexacao
- IndexStatus enum: Pending, Processing, Completed, Failed
- StartProjectIndexCommand/Handler inicia job de indexacao
- GetIndexStatusQuery/Handler retorna status do job mais recente
- Protecao contra duplicate index jobs (InvalidOperationException se ja em andamento)
- Endpoint POST /api/v1/projects/{id}/index para iniciar indexacao
- Endpoint GET /api/v1/projects/{id}/index/status para consultar status
- ProjectIndexJobRepository com GetPendingOrProcessingByProjectIdAsync, UpdateAsync, GetPendingJobsAsync
- Configuration EntityFramework para project_index_jobs table
- ProjectIndexerService (BackgroundService) processa jobs pendentes com polling de 10s
  - Transiciona jobs: Pending -> Processing -> Completed (ou Failed em caso de erro)
  - Dual-constructor pattern: producao (IServiceScopeFactory) e teste (dependencias diretas)
  - InternalsVisibleTo permite acesso ao construtor interno nos testes unitarios
  - MemoryEntity nao tem ProjectId — indexacao marca jobs como completed com 0 memories processadas
- Registrado como AddHostedService<ProjectIndexerService> no Program.cs
- 14 testes de dominio (ProjectIndexJobEntityTests): Create, MarkAsProcessing, MarkAsCompleted, MarkAsFailed, SetTotalMemories
- 5 testes de servico (ProjectIndexerServiceTests): pending jobs, no jobs, error handling, state transitions, multiple jobs
- 7 testes de integracao (ProjectIndexEndpointTests): start index, conflict, 404, status, auth

#### Task 01 - Autenticacao API Key
- Implementado UserEntity com hash BCrypt para seguranca de API Keys
- API Key via header `X-Api-Key`
- ValidateApiKeyHandler como Query Handler para validacao
- ApiKeyMiddleware protege todos os endpoints exceto `/api/v1/auth/*`
- Middleware injeta UserId no HttpContext.Items para acesso posterior
- UserRepository usa BCrypt.Verify para busca (carrega todos os usuarios e verifica cada um)
  - BCrypt hashes sao nao-deterministicos: nao e possivel fazer WHERE hash = @hash
  - Aceitavel para sistema self-hosted com poucos usuarios
- UserEntityConfiguration mapeia para tabela `users` schema `mnemosyne` com snake_case
- Testes de integracao via WebApplicationFactory<Program> com InMemory DB
  - Necessario substituir DbContext completo (remover Npgsql provider para evitar conflito dual-provider)
  - Necessario registrar stub de IEmbeddingService (dependencia de Task 4 nao registrada)
  - Necessario ignorar coluna Embedding (pgvector Vector nao suportado pelo InMemory provider)
  - Necessario registrar handlers de Task 3 (StartProjectIndex, GetIndexStatus) para evitar falha de inferencia de parametros
- Bug pre-existente corrigido: MemoryEndpoints.MapGet("/search") usava `handler = null!` como default
  - Minimal API nao infere corretamente servicos DI com valores default
  - Corrigido com `[FromServices]` explicito

#### Task 02 - CRUD de Projetos
- ProjectEntity com isolamento logico por UserId
- Repository pattern com IProjectRepository (AddAsync, GetByIdAsync, GetByUserIdAsync, UpdateAsync, DeleteAsync)
- CRUD completo com 5 endpoints:
  - POST /api/v1/projects (Create)
  - GET /api/v1/projects?userId=... (List com [AsParameters])
  - GET /api/v1/projects/{id} (Get by Id)
  - PUT /api/v1/projects/{id} (Update)
  - DELETE /api/v1/projects/{id} (Delete)
- Metodo Update(name, description) em ProjectEntity para atualizacao controlada
- Cada acao em feature folder separado: CreateProject, GetProject, ListProjects, UpdateProject, DeleteProject
- Handlers validam inputs e lancam ArgumentException (400) ou KeyNotFoundException (404)
- ListProjectsRequest usa [AsParameters] para bind de query string em Minimal API
- Configuracao EF Core para Projects com indice em UserId
- 13 testes unitarios + 12 testes de integracao cobrindo todos os endpoints
- Testes de integracao verificam middleware de autenticacao (401 sem API Key)
- Teste de Delete verifica exclusao real (GET apos DELETE retorna 404)

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
