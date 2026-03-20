# ADR 001: Memory Domain Model

## Status
Accepted - 2026-03-19

## Context
Mnemosyne is a Memory & Context Intelligence Platform that needs a core domain model to represent stored memories. The domain layer must have no external dependencies and represent the fundamental aggregate of the system.

## Decision
Implemented `MemoryEntity` as the core aggregate and `MemoryType` as an enum with 4 values.

### MemoryType Enum
```csharp
public enum MemoryType
{
    Note,
    Decision,
    Preference,
    Context
}
```

### MemoryEntity Class
- **Id**: GUID primary identifier
- **Content**: Non-empty string containing the memory content
- **Type**: MemoryType enum value
- **CreatedAt**: UTC timestamp of creation

### Factory Method
`MemoryEntity.Create(string content, MemoryType type)` enforces:
- Content cannot be null, empty, or whitespace (throws ArgumentException)

## Consequences
**Positive:**
- Clear, minimal domain model
- Guard clauses prevent invalid state
- Factory pattern encapsulates creation rules
- No external dependencies in Domain layer

**Negative:**
- None identified

## Notes
- Following TDD approach: tests written first (RED), then implementation (GREEN)
- Minimal implementation only - more rules will be added as needed
