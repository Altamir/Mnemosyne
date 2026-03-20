# Learning Log

## Fase 2 - Autenticacao e Projetos

### 2026-03-20

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
