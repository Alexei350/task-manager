# Configuração de Segurança - Task Manager

## Variáveis de Ambiente Sensíveis

Este projeto utiliza variáveis de ambiente para armazenar credenciais e dados sensíveis. **NUNCA** commite arquivos `.env` ou configurações com valores reais no repositório.

### Backend (.NET)

#### Configuração em Desenvolvimento

Use `dotnet user-secrets` para armazenar secrets locais:

```bash
# Navegar até apps/backend/TaskManager
cd apps/backend/TaskManager

# Definir JWT Key
dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-aleatoria-de-32-caracteres-minimo"

# Definir Google Client ID
dotnet user-secrets set "GoogleClientId" "seu-google-client-id"

# Definir senha do banco de dados
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=5432;Database=task_manager;User Id=postgres;Password=sua_senha_segura"
```

#### Configuração em Produção/Docker

Use variáveis de ambiente do sistema ou orquestrador (Docker Compose, Kubernetes, etc):

```bash
# Docker Compose
export Jwt__Key="sua-chave-secreta"
export GoogleClientId="seu-google-client-id"
export ConnectionStrings__DefaultConnection="Server=db;..."
docker-compose up
```

### Frontend (React/Vite)

Crie um arquivo `.env.local` baseado em `.env.example`:

```bash
# .env.local
VITE_API_BASE_URL=http://localhost:8000
VITE_GOOGLE_CLIENT_ID=seu-google-client-id
```

### MCP Server

Crie um arquivo `.env` baseado em `.env.example`:

```bash
# .env
API_KEY=tm_sk_sua-chave-api
API_URL=https://seu-dominio.com.br
```

## Boas Práticas de Segurança

1. **Nunca commite arquivos `.env`** - Estão no `.gitignore` por uma razão
2. **Use `dotnet user-secrets` em desenvolvimento** - Mais seguro que variáveis de ambiente
3. **Gere chaves fortes** - Use `openssl rand -base64 32` para gerar JWT Keys
4. **Rotação de segredos** - Rotinizar mudança de API Keys e senhas
5. **Diferencie ambientes** - Use valores diferentes para dev/staging/produção
6. **Use CI/CD Secrets** - GitHub Actions, GitLab CI, etc têm gerenciadores de secrets

## Verificação de Segurança

Para verificar se há secrets no repositório:

```bash
# Procurar por padrões comuns de secrets
git log --all --full-history -S "password" -- .
git log --all --full-history -S "secret" -- .
git log --all --full-history -S "api_key" -- .
```

## Referências

- [Microsoft - Safe storage of app secrets in development](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [12 Factor App - Store config in the environment](https://12factor.net/config)
