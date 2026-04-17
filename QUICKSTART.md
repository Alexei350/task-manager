# 🚀 Quick Start

Comece a desenvolver no Task Manager em minutos!

## ⚡ Setup Rápido

### 1. Clone e Abra

```bash
git clone https://github.com/Alexei350/task-manager.git
cd task-manager
code task-manager.code-workspace
```

### 2. Configurar Secrets e Variáveis de Ambiente

**IMPORTANTE:** Este projeto utiliza variáveis de ambiente para armazenar credenciais sensíveis. Leia [docs/SECURITY.md](./docs/SECURITY.md) para detalhes.

```bash
# Criar .env baseado no exemplo
cp .env.example .env

# Editar .env e preencher com valores reais:
# - POSTGRES_PASSWORD
# - JWT_KEY (gerar com: openssl rand -base64 32)
# - VITE_GOOGLE_CLIENT_ID (se usando Google OAuth)
nano .env
```

**Backend - Configurar User Secrets em desenvolvimento:**
```bash
cd apps/backend/TaskManager
dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-de-32-caracteres-minimo"
dotnet user-secrets set "GoogleClientId" "seu-google-client-id"
```

### 3. Backend

```bash
# Terminal 1: Banco de dados
cd apps/backend
docker-compose up -d

# Terminal 2: API
dotnet restore
dotnet run --project TaskManager/TaskManager.csproj
```

✅ API rodando em: http://localhost:5000  
📚 Swagger: http://localhost:5000/swagger

### 4. Frontend

```bash
cd apps/frontend
npm install

# Criar .env.local baseado em .env.example
cp .env.example .env.local
# (Google Client ID já configurado no backend)

# Ambiente de desenvolvimento
npm run dev

# Testes rápidos
npm run test:ci
```

## 🧪 Rodar Testes

```bash
# Backend
cd apps/backend
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📖 Próximos Passos

- 📘 Leia o [Guia de Desenvolvimento](./docs/guides/development.md)
- 🏗️ Veja a [Estrutura do Projeto](./docs/STRUCTURE.md)
- 🎯 Confira as [Decisões de Arquitetura](./docs/decisions/)

## 💡 Comandos Úteis

```bash
# Ver logs do banco
cd apps/backend && docker-compose logs -f postgres

# Criar migration
cd apps/backend/TaskManager
dotnet ef migrations add NomeDaMigration

# Limpar builds
cd apps/backend
dotnet clean && rm -rf */bin */obj

# Rodar container da API
cd apps/backend
docker build -t task-manager .
docker run -p 5000:8080 task-manager
```

## 🆘 Problemas?

- Porta 5000 em uso? `lsof -i :5000` e `kill -9 <PID>`
- Banco não conecta? `docker-compose restart postgres`
- Builds falhando? `dotnet clean && dotnet restore`

Mais ajuda em: [Troubleshooting](./docs/guides/development.md#-problemas-comuns)
