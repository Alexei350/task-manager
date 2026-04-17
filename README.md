# Task Manager - Monorepo

- Backend: [![Backend Build](https://jenkins.alexei.dev.br/job/task-manager-backend/job/main/badge/icon)](https://jenkins.alexei.dev.br/job/task-manager-backend/job/main/)
- Frontend: [![Frontend Build](https://jenkins.alexei.dev.br/job/task-manager-frontend/job/main/badge/icon)](https://jenkins.alexei.dev.br/job/task-manager-frontend/job/main/)

Sistema de gerenciamento de tarefas com arquitetura monorepo.

## 📁 Estrutura do Projeto

```
task-manager/
├── apps/
│   ├── backend/           # API .NET 10
│   └── frontend/          # Aplicação front-end (a ser implementado)
├── packages/              # Pacotes compartilhados
├── docs/                  # Documentação
└── README.md
```

## 🚀 Aplicações

### Backend
API RESTful desenvolvida em .NET 10 com PostgreSQL.

**Tecnologias:**
- .NET 10
- Entity Framework Core
- PostgreSQL 17
- JWT Authentication
- Docker

[Documentação do Backend](./apps/backend/README.md)

### Frontend
Interface de usuário em React + Vite para consumo da API.

**Tecnologias:**
- React 19 + TypeScript
- Vite
- Vitest + Testing Library
- Docker (Nginx)

## 📦 Pacotes Compartilhados

Biblioteca de componentes, tipos e utilitários compartilhados entre front-end e back-end (futura implementação).

## 🏗️ Como Começar

📖 **[Quick Start Guide](./QUICKSTART.md)** - Configure o ambiente em minutos  
📘 **[Guia de Desenvolvimento](./docs/guides/development.md)** - Documentação completa  
🏗️ **[Estrutura do Projeto](./docs/STRUCTURE.md)** - Organização de pastas e arquivos

### Pré-requisitos
- Docker e Docker Compose
- .NET 10 SDK
- Node.js 20+ (para o frontend, quando implementado)

### Backend

```bash
cd apps/backend
docker-compose up -d
dotnet restore
dotnet run --project TaskManager/TaskManager.csproj
```

### Frontend

```bash
cd apps/frontend
npm install
npm run dev
```

## 🧪 Testes

### Backend
```bash
cd apps/backend
dotnet test TaskManager.UnitTests/TaskManager.UnitTests.csproj --collect:"XPlat Code Coverage"
```

### Frontend
```bash
cd apps/frontend
npm run test:ci
```

## 📊 CI/CD

Cada aplicação possui seu próprio pipeline Jenkins:

- **Backend** (`apps/backend/Jenkinsfile`): 
  - Testes automatizados
  - Análise de cobertura de código
  - Build de imagem Docker
  - Deploy automático no servidor

- **Frontend** (`apps/frontend/Jenkinsfile`):
  - npm ci
  - Testes com Vitest
  - Build do bundle
  - Build e push da imagem Docker

📖 [Documentação completa do CI/CD](./docs/CICD.md)

## � Segurança

Este projeto segue boas práticas de segurança:

- **Variáveis de Ambiente:** Todas as credenciais são configuradas via variáveis de ambiente, nunca hardcodeadas
- **User Secrets:** Desenvolvimento local usa `dotnet user-secrets` para armazenar dados sensíveis
- **`.env` Ignorado:** Arquivo `.env` está no `.gitignore` para evitar commits acidentais
- **Secrets no Git:** Verifique [docs/SECURITY.md](./docs/SECURITY.md) para boas práticas

⚠️ **IMPORTANTE:** Nunca commite arquivos `.env`, `appsettings.json` ou `launchSettings.json` com valores reais no repositório!

## �📝 Licença

Projeto privado.

## 👤 Autor

Alexei Secretti
