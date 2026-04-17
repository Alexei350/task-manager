# Estrutura do Monorepo Task Manager

Visão geral da organização de pastas e arquivos do projeto.

## 📂 Estrutura Atual

```
task-manager/                                    # Raiz do monorepo
│
├── apps/                                        # Aplicações
│   ├── backend/                                 # API .NET 10
│   │   ├── TaskManager/                         # Projeto principal
│   │   │   ├── Controllers/                     # Endpoints REST
│   │   │   ├── Services/                        # Lógica de negócio
│   │   │   ├── Repository/                      # Camada de dados
│   │   │   ├── Models/                          # DTOs, Entities, Enums
│   │   │   ├── Context/                         # EF Core DbContext
│   │   │   ├── Migrations/                      # Migrations do banco
│   │   │   ├── Utils/                           # Extensões e utilitários
│   │   │   ├── Mapping/                         # Configurações de mapeamento
│   │   │   └── Resources/                       # Arquivos de i18n
│   │   ├── TaskManager.UnitTests/               # Testes unitários
│   │   │   ├── Service/                         # Testes de serviços
│   │   │   ├── Repository/                      # Testes de repositórios
│   │   │   └── Utils/                           # Testes de utilitários
│   │   ├── scripts/                             # Scripts de automação
│   │   │   ├── deploy.sh                        # Deploy manual
│   │   │   ├── migrate.sh                       # Migrations local
│   │   │   └── migrate-server.sh                # Migrations servidor
│   │   ├── docker-compose.yaml                  # PostgreSQL container
│   │   ├── Dockerfile                           # Build da API
│   │   ├── Jenkinsfile                          # Pipeline CI/CD
│   │   ├── TaskManager.sln                      # Solution .NET
│   │   └── README.md                            # Documentação do backend
│   │
│   └── frontend/                                # Aplicação front-end em React
│       ├── src/                                 # Componentes e contexto
│       ├── Jenkinsfile                          # Pipeline do frontend
│       ├── Dockerfile                           # Build estático + Nginx
│       └── README.md                            # Documentação do frontend
│
├── packages/                                    # Código compartilhado (futuro)
│   └── README.md                                # Guia de pacotes
│       # Estrutura planejada:
│       # ├── shared-types/                      # Tipos TypeScript
│       # ├── ui-components/                     # Componentes reutilizáveis
│       # ├── utils/                             # Funções utilitárias
│       # └── api-client/                        # Cliente HTTP
│
├── docs/                                        # Documentação
│   ├── decisions/                               # ADRs (Architecture Decision Records)
│   │   └── 001-monorepo-structure.md            # Decisão sobre monorepo
│   ├── guides/                                  # Guias práticos
│   │   └── development.md                       # Setup e desenvolvimento
│   └── README.md                                # Índice da documentação
│       # Estrutura planejada:
│       # ├── architecture/                      # Diagramas e arquitetura
│       # ├── api/                               # Documentação de endpoints
│       # └── troubleshooting/                   # Resolução de problemas
│
├── .vscode/                                     # Configurações do VS Code
├── .git/                                        # Repositório Git
├── .env                                         # Variáveis de ambiente locais
├── .gitignore                                   # Arquivos ignorados pelo Git
├── task-manager.code-workspace                  # Workspace multi-folder do VS Code
└── README.md                                    # Documentação principal
```

## 📊 Estatísticas do Projeto

### Backend
- **Linguagem:** C# (.NET 10)
- **Testes:** 95 testes unitários
- **Cobertura:** 36.2%
- **Endpoints:** 15+ rotas REST
- **Banco:** PostgreSQL 17

### Frontend
- **Status:** Implementado
- **Stack:** React + Vite + Vitest

### Documentação
- **ADRs:** 1 decisão documentada
- **Guias:** 1 guia de desenvolvimento
- **READMEs:** 5 arquivos de documentação

## 🎯 Princípios de Organização

### 1. Separação por Contexto
- Cada aplicação (`backend`, `frontend`) é independente
- Podem ser desenvolvidas, testadas e deployadas separadamente
- Compartilham código através da pasta `packages/`

### 2. Colocação por Feature
- Código organizado por funcionalidade, não por tipo de arquivo
- Exemplo: `TaskService`, `TaskController`, `TaskRepository` relacionados

### 3. Documentação Próxima ao Código
- Cada app tem seu próprio README
- Documentação técnica centralizada em `docs/`
- Decisões arquiteturais registradas em ADRs

### 4. Configuração Centralizada
- Workspace do VS Code na raiz
- Git na raiz do monorepo
- CI/CD pode orquestrar builds de múltiplas apps

## 🔄 Fluxo de Trabalho

### Desenvolvimento
```bash
# Clonar repositório
git clone <url>

# Abrir workspace
code task-manager.code-workspace

# Backend: trabalhar em apps/backend
# Frontend: trabalhar em apps/frontend
# Pacotes: trabalhar em packages/
```

### Build e Deploy
```bash
# Backend
cd apps/backend
dotnet build
dotnet test

# Frontend (futuro)
cd apps/frontend
npm run build
npm test

# Deploy
# Jenkins detecta mudanças e faz deploy automaticamente
```

## 📝 Convenções

### Nomenclatura
- **Pastas:** lowercase-kebab-case
- **Arquivos C#:** PascalCase
- **Arquivos TS/JS:** camelCase ou kebab-case
- **Componentes:** PascalCase

### Estrutura de Commits
```
<tipo>(<escopo>): <descrição>

feat(backend): adiciona endpoint de tarefas
fix(frontend): corrige validação de formulário
docs: atualiza guia de desenvolvimento
test(backend): adiciona testes de usuário
```

## 🚀 Evolução Futura

### Curto Prazo
- [ ] Criar pacote de tipos compartilhados
- [ ] Adicionar documentação de API

### Médio Prazo
- [ ] Criar biblioteca de componentes UI
- [ ] Implementar cliente de API compartilhado
- [ ] Adicionar testes E2E

### Longo Prazo
- [ ] Considerar ferramentas de monorepo (Nx, Turborepo)
- [ ] Implementar cache de builds
- [ ] Otimizar CI/CD para builds incrementais

## 🔗 Referências

- [Monorepo no Google](https://research.google/pubs/pub45424/)
- [Estrutura de Projetos .NET](https://docs.microsoft.com/dotnet/architecture/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
