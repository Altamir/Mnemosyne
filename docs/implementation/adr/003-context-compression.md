# ADR-003: Estrategia de Compressao de Contexto

## Status
Aceito

## Contexto
Mnemosyne precisa comprimir texto/codigo para otimizar contexto enviado a LLMs. A compressao deve ser extensivel para suportar multiplas estrategias no futuro (code_structure, conversation_summary, semantic_dedup, hierarchical), mas inicialmente apenas CodeStructure sera implementada.

## Decisoes

### 1. Strategy Pattern via DI com IEnumerable

**Decisao:** Usar `ICompressionStrategy` resolvido via `IEnumerable<ICompressionStrategy>` no handler, sem factory class.

**Alternativas consideradas:**
- **Factory class (ICompressionStrategyFactory):** Adiciona camada desnecessaria de indirecton para o caso atual
- **Dictionary<enum, strategy> registrado no DI:** Menos idiomatico, acoplamento com container

**Justificativa:**
- Extensivel: adicionar nova estrategia = nova classe + registro no DI
- Idiomatico: usa resolucao nativa do container de DI
- Simples: handler recebe todas as estrategias e filtra por `StrategyName`
- Sem factory intermediaria desnecessaria

**Implementacao:**
```csharp
// Interface
public interface ICompressionStrategy
{
    string StrategyName { get; }
    Task<CompressionResult> CompressAsync(string content, double targetRatio, CancellationToken ct);
}

// Handler resolve por nome
var strategy = _strategies.FirstOrDefault(s =>
    s.StrategyName.Equals(command.Strategy.ToString(), StringComparison.OrdinalIgnoreCase))
    ?? throw new ArgumentException($"Estrategia de compressao '{command.Strategy}' nao encontrada.");
```

### 2. Enum CompressionStrategy como Seletor

**Decisao:** Usar enum `CompressionStrategy` no command para selecionar a estrategia.

**Justificativa:**
- Type-safe: compilador valida valores
- Extensivel: adicionar novo valor ao enum + nova classe
- Serializavel: enum mapeia naturalmente para JSON string/int

### 3. Regex com Source Generator para Compressao

**Decisao:** Usar `[GeneratedRegex]` para todos os patterns de compressao na CodeStructureCompressionStrategy.

**Justificativa:**
- Performance: regex compilado em tempo de build
- AOT-friendly: compativel com Native AOT compilation
- Stateless: permite registro como Singleton

### 4. targetRatio Configuravel com Default 0.7

**Decisao:** targetRatio e parametro do command com default 0.7, validado entre 0 (exclusivo) e 1 (exclusivo).

**Justificativa:**
- Flexibilidade: clientes podem ajustar nivel de compressao
- Seguranca: validacao impede valores invalidos
- Convencao: 0.7 significa ~70% de reducao (alinhado com th0th tools)

## Consequencias

### Positivas
- Facilmente extensivel para novas estrategias (conversation_summary, semantic_dedup, etc.)
- Codigo limpo sem factory class desnecessaria
- Alta performance com regex source-generated
- 100% de cobertura de testes nos arquivos de compressao

### Negativas
- Resolucao linear (FirstOrDefault) — O(n) com n = numero de estrategias registradas
- Se muitas estrategias forem adicionadas, considerar Dictionary lookup

## Arquivos Relacionados
- `src/Mnemosyne.Domain/Enums/CompressionStrategy.cs`
- `src/Mnemosyne.Domain/Services/ICompressionStrategy.cs`
- `src/Mnemosyne.Domain/Services/CompressionResult.cs`
- `src/Mnemosyne.Application/Features/Compress/CompressContext/CompressContextCommand.cs`
- `src/Mnemosyne.Application/Features/Compress/CompressContext/CompressContextHandler.cs`
- `src/Mnemosyne.Infrastructure/Compression/CodeStructureCompressionStrategy.cs`
- `src/Mnemosyne.Api/Endpoints/CompressEndpoints.cs`

## Data
2026-03-20
