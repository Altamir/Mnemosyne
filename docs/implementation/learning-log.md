# Learning Log

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
