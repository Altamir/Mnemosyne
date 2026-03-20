# Task 06 - gRPC para Search, Index e Compress

## Objetivo
Adicionar contratos Protobuf e servicos gRPC para operacoes de alta performance.

## Dependencias
- Requer Task 03, Task 04 e Task 05 da Fase 2 concluidas.

## Arquivos
- Create: `src/Mnemosyne.Api/Protos/search.proto`
- Create: `src/Mnemosyne.Api/Protos/index.proto`
- Create: `src/Mnemosyne.Api/Protos/compress.proto`
- Create: `src/Mnemosyne.Api/GrpcServices/SearchGrpcService.cs`
- Create: `src/Mnemosyne.Api/GrpcServices/IndexGrpcService.cs`
- Create: `src/Mnemosyne.Api/GrpcServices/CompressGrpcService.cs`
- Modify: `src/Mnemosyne.Api/Mnemosyne.Api.csproj`
- Modify: `src/Mnemosyne.Api/Program.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Grpc/SearchGrpcTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Grpc/IndexGrpcTests.cs`
- Test: `tests/Mnemosyne.IntegrationTests/Grpc/CompressGrpcTests.cs`

## Planejamento (TDD)
1. RED: escrever testes de contrato e comportamento gRPC para os 3 servicos.
2. VERIFY RED: executar e confirmar falha por ausencia de contratos/servicos.
3. GREEN: criar protos, gerar codigo e implementar servicos minimos.
4. VERIFY GREEN: executar testes gRPC e validar respostas.
5. REFACTOR: ajustar DTO mapping entre REST/Application/gRPC.
6. DOCS: atualizar logs e ADR de transporte gRPC para alto throughput.
7. COMMIT: `feat: adiciona servicos grpc para search index e compress`.

## Comandos principais
```bash
dotnet add src/Mnemosyne.Api/Mnemosyne.Api.csproj package Grpc.AspNetCore
dotnet build src/Mnemosyne.Api/Mnemosyne.Api.csproj
dotnet test tests/Mnemosyne.IntegrationTests --filter "SearchGrpcTests|IndexGrpcTests|CompressGrpcTests"
```
