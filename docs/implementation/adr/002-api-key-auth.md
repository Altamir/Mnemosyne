# ADR-002: Estrategia de Autenticacao por API Key

## Status
Aceito

## Contexto
Sistema Mnemosyne precisa de autenticacao simples para organizar memorias por usuario. Requer mecanismo de seguranca para proteger endpoints da API.

## Decisoes

### 1. Autenticacao por API Key com Hash BCrypt

**Decisao:** Armazenar hash BCrypt da API Key no banco, nunca a chave em texto plano.

**Justificativa:**
- BCrypt e resistente a rainbow tables e brute force
- Hashing com salt embutido
- Comparacao segura via BCrypt.Verify

**Implementacao:**
```csharp
// Criacao
ApiKeyHash = BCrypt.Net.BCrypt.HashPassword(apiKey);

// Validacao
public bool ValidateApiKey(string apiKey)
{
    return BCrypt.Net.BCrypt.Verify(apiKey, ApiKeyHash);
}
```

### 2. Middleware para Validacao Automatica

**Decisao:** ApiKeyMiddleware valida header `X-Api-Key` em todas as requisicoes.

**Justificativa:**
- Validacao centralizada
- Proteger endpoints por configuracao, não por atributo
- UserId injetado no HttpContext para uso posterior

**Rota excluida:** `/api/v1/auth/*` - permite validacao manual de credenciais

### 3. Header de API Key

**Decisao:** Usar header `X-Api-Key` em vez de Authorization Bearer.

**Justificativa:**
- Simplicidade para clientes
- API Key e um token opaco, não JWT
- Evita confusao com OAuth Bearer tokens

## Consequencias

### Positivas
- Implementacao simples e direta
- Seguranca adequada para uso interno
- Sem dependencia de servicos externos (como Auth0)

### Negativas
- Gerenciamento de API Keys e旋转 e problema do cliente
- Não ha suporte a expiracao de tokens nativa
- Não ha suporte a scopes/permissoes granulares

## Arquivos Relacionados
- `src/Mnemosyne.Domain/Entities/UserEntity.cs`
- `src/Mnemosyne.Api/Middleware/ApiKeyMiddleware.cs`
- `src/Mnemosyne.Application/Features/Auth/ValidateApiKey/`

## Data
2026-03-20