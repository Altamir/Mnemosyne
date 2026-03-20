# Task 02 - Modelo de dominio de memoria

## Objetivo
Implementar `MemoryEntity` e `MemoryType` com regras minimas de dominio, usando TDD estrito.

## Dependencias
- Requer Task 01 concluida.

## Arquivos
- Create: `src/Mnemosyne.Domain/Entities/MemoryEntity.cs`
- Create: `src/Mnemosyne.Domain/Enums/MemoryType.cs`
- Test: `tests/Mnemosyne.UnitTests/Domain/MemoryEntityTests.cs`

## Planejamento (TDD)
1. RED: escrever testes para criacao valida e validacao de conteudo vazio.
2. VERIFY RED: executar `MemoryEntityTests` e confirmar falha esperada.
3. GREEN: implementar `MemoryType` e `MemoryEntity.Create(...)` com guard clauses minimas.
4. VERIFY GREEN: executar testes e confirmar sucesso.
5. REFACTOR: remover duplicacao em fixtures/helpers de teste.
6. DOCS: criar ADR do modelo de memoria + atualizar learning/error-fix logs.
7. COMMIT: `feat: adiciona modelo de dominio de memoria`.

## Comandos principais
```bash
dotnet test tests/Mnemosyne.UnitTests --filter "MemoryEntityTests"
```
