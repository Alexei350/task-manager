# Frontend (Vite + React)

[![Build Status](https://jenkins.alexei.dev.br/job/task-manager-frontend/job/main/badge/icon)](https://jenkins.alexei.dev.br/job/task-manager-frontend/job/main/)

Interface React moderna que consome a API .NET para autenticação via JWT e gerenciamento completo de tarefas.

## 🚀 Stack

- **Build:** Vite + TypeScript
- **UI:** React 19 (sem roteador; SPA simples)
- **Testes:** Vitest + React Testing Library + jsdom
- **Pipeline:** Jenkinsfile dedicado (npm ci → test → build → docker)
- **Container:** Docker (build estático + Nginx)

## ✨ Funcionalidades

### 🔐 Autenticação
- Login com JWT (autenticação via API .NET)
- Login Social com Google
- Registro de novos usuários
- Persistência de sessão no localStorage

### 📋 Gerenciamento de Tarefas

#### Visualizações
- **Kanban Board:** Visualização em quadro Kanban com colunas por status
  - Pendente
  - Em Progresso
  - Concluída
  - Pausada
  - Cancelada
- **Tabela:** Visualização tradicional em tabela com paginação

#### Operações
- ✅ Criar tarefas via modal intuitivo
- ✏️ Editar tarefas existentes
- 🗑️ Excluir tarefas (com confirmação)
- 🔄 Alterar status via drag-and-drop (Kanban) ou dropdown (Tabela)
- 🔍 Filtrar tarefas por descrição
- 📄 Paginação de resultados

### 🎨 Experiência do Usuário
- Interface moderna e responsiva
- Modais para formulários (melhor UX que formulários inline)
- Estados de carregamento e feedback visual
- Animações suaves
- Design mobile-friendly
- Mensagens de sucesso e erro contextualizadas

## ⚙️ Configuração

```bash
cd apps/frontend
npm install
```

### Variáveis de ambiente

As variáveis de ambiente podem ser configuradas de duas formas:

#### 1. Desenvolvimento Local (.env)
Crie um arquivo `.env` na raiz de `apps/frontend/`:

```env
VITE_API_BASE_URL=http://localhost:30000
VITE_GOOGLE_CLIENT_ID=seu_client_id_do_google
```

#### 2. Docker (Build-time)
Configure as variáveis no arquivo `.env` na **raiz do projeto** (não commitar):

```env
VITE_API_BASE_URL=http://localhost:30000
VITE_GOOGLE_CLIENT_ID=seu_google_client_id
```

O Docker Compose irá passar essas variáveis como **argumentos de build**.

#### 3. Jenkins (CI/CD)
O Jenkinsfile usa **credentials do Jenkins** para injetar as variáveis:
- `google-client-id`: Client ID do Google OAuth
- `frontend-api-url`: URL da API (ex: https://api.alexei.dev.br)

Configure essas credentials no Jenkins em: **Manage Jenkins → Credentials → Add Credentials**

## 🧭 Como rodar

```bash
npm run dev        # Ambiente de desenvolvimento (porta 5173)
npm run preview    # Pré-visualizar build
```

## ✅ Testes

```bash
npm test           # Vitest em modo watch
npm run test:ci    # Execução em modo CI com cobertura
```

### Cobertura de Testes
- ✅ Componentes de autenticação (Login, Register)
- ✅ Dashboard de tarefas
- ✅ Formulário de tarefas
- ✅ Kanban Board
- ✅ Modal
- ✅ Integração com API

## 🎯 Lint

```bash
npm run lint       # ESLint com configuração TypeScript + React
```

## 📦 Build

```bash
npm run build
```

## 🐳 Docker

### Build da imagem com variáveis
```bash
cd apps/frontend
docker build \
  --build-arg VITE_API_BASE_URL=http://localhost:30000 \
  --build-arg VITE_GOOGLE_CLIENT_ID=seu_google_client_id \
  -t task-manager-frontend .
```

### Via Docker Compose (recomendado)
Configure as variáveis no arquivo `.env` na raiz do projeto:

```env
VITE_API_BASE_URL=http://localhost:30000
VITE_GOOGLE_CLIENT_ID=seu_google_client_id
```

Depois execute:
```bash
docker compose build task-manager-frontend
docker compose up -d task-manager-frontend
```

## 🔄 Pipeline (Jenkins)

1. `npm ci`
2. `npm run test:ci`
3. `npm run build`
4. Build + push da imagem `alexei350/task-manager-frontend`

## 📐 Arquitetura

```
src/
├── components/         # Componentes React
│   ├── LoginForm.tsx
│   ├── RegisterForm.tsx
│   ├── TaskDashboard.tsx
│   ├── TaskForm.tsx
│   ├── KanbanBoard.tsx  # Visualização Kanban
│   ├── Modal.tsx        # Modal reutilizável
│   └── StatusBadge.tsx
├── context/            # Context API
│   ├── AuthContext.tsx
│   └── useAuth.ts
├── services/           # Integrações com API
│   └── api.ts
├── utils/              # Utilitários
│   └── status.ts
└── test/               # Testes unitários
    ├── App.test.tsx
    ├── KanbanBoard.test.tsx
    └── Modal.test.tsx
```

## 🔗 Links

- [Documentação da API](../backend/README.md)
- [Monorepo Principal](../../README.md)
