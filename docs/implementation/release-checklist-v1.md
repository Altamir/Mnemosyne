# Release Checklist v1.0

## Pré-verificações

- [ ] Todos os testes unitários passam
- [ ] Todos os testes de integração passam
- [ ] Build completa em Release sem warnings

## Build & Test

```bash
# Build em Release
dotnet build -c Release

# Executar todos os testes
dotnet test -c Release

# Executar testes unitários apenas
dotnet test tests/Mnemosyne.UnitTests -c Release

# Executar testes de integração apenas
dotnet test tests/Mnemosyne.IntegrationTests -c Release
```

## Qualidade de Código

- [ ] Código compila sem erros
- [ ] Sem warnings de compilação
- [ ] Nomenclatura consistente com convenções .NET
- [ ] Sem code smells óbvios

## Health Endpoints

- [ ] `GET /health/live` retorna 200 OK
- [ ] `GET /health/ready` retorna 200 quando banco conectado
- [ ] `GET /health/ready` retorna 503 quando banco inacessível

## Documentação

- [ ] README.md atualizado com quick start
- [ ] Release notes documentados
- [ ] ADR de domínio revisado

## Infraestrutura

- [ ] docker-compose.yml valida e funcional
- [ ] PostgreSQL com pgvector configurado
- [ ] Connection string correta no docker-compose

## Commits

- [ ] Histórico de commits limpo e descritivo
- [ ] Ultimo commit: `chore: finaliza baseline v1 com testes e documentacao`
