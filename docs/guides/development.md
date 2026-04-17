# Guia de Desenvolvimento

Este guia ajudará você a configurar o ambiente de desenvolvimento do Task Manager.

## 📋 Pré-requisitos

### Obrigatórios
- **Git** - Controle de versão
- **Docker** e **Docker Compose** - Containers
- **.NET 10 SDK** - Backend
- **Node.js 20+** e **npm/yarn** - Frontend (quando implementado)
- **VS Code** (recomendado) - IDE

### Recomendados
- **DBeaver** ou **pgAdmin** - Gerenciamento de banco de dados
- **Postman** ou **Insomnia** - Teste de APIs
- **Git Extensions** ou **GitKraken** - GUI para Git

## 🚀 Setup Inicial

### 1. Clone o Repositório

```bash
git clone https://github.com/Alexei350/task-manager.git
cd task-manager
```

### 2. Abra o Workspace no VS Code

```bash
code task-manager.code-workspace
```

Este workspace configurado facilita a navegação entre backend, frontend e pacotes.

## 🔧 Backend (.NET)

### Setup

```bash
cd apps/backend

# Restaurar dependências
dotnet restore

# Subir banco de dados PostgreSQL
docker-compose up -d postgres

# Aplicar migrations
cd TaskManager
dotnet ef database update
cd ..

# Rodar aplicação
dotnet run --project TaskManager/TaskManager.csproj
```

A API estará disponível em: `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

### Testes

```bash
cd apps/backend

# Rodar todos os testes
dotnet test

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório de cobertura
reportgenerator \
  -reports:"**/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/CoverageReport" \
  -reporttypes:"Html;TextSummary"

# Abrir relatório
open TestResults/CoverageReport/index.html  # macOS
xdg-open TestResults/CoverageReport/index.html  # Linux
```

### Criar Migration

```bash
cd apps/backend/TaskManager
dotnet ef migrations add NomeDaMigration
```

### Estrutura de Pastas

```
apps/backend/
├── TaskManager/              # Projeto principal
│   ├── Controllers/         # Endpoints da API
│   ├── Services/            # Lógica de negócio
│   ├── Repository/          # Acesso a dados
│   ├── Models/              # DTOs e Entities
│   ├── Context/             # EF Core Context
│   └── Utils/               # Utilitários
├── TaskManager.UnitTests/   # Testes unitários
├── docker-compose.yaml      # Containers (PostgreSQL)
└── Dockerfile               # Build da aplicação
```

## 🎨 Frontend (React + Vite)

### Setup

```bash
cd apps/frontend
npm install

# Opcional: definir a URL da API (padrão: http://localhost:30000)
echo "VITE_API_BASE_URL=http://localhost:30000" > .env
```

### Rodar

```bash
npm run dev         # http://localhost:5173
npm run preview     # serve o build
```

### Testes

```bash
npm test            # Vitest em modo watch
npm run test:ci     # Execução em modo CI com cobertura
```

## 📦 Pacotes Compartilhados

Quando criar pacotes compartilhados:

```bash
cd packages/nome-do-pacote

# Instalar dependências
npm install

# Build
npm run build

# Publicar localmente
npm link
```

Usar em outro projeto:

```bash
cd apps/frontend
npm link nome-do-pacote
```

## 🐳 Docker

### Backend

```bash
cd apps/backend

# Build da imagem
docker build -t task-manager-api .

# Rodar container
docker run -p 5000:8080 task-manager-api
```

### Full Stack (quando frontend estiver pronto)

```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

## 🔍 Debugging

### Backend (VS Code)

1. Abra a pasta `apps/backend` no VS Code
2. Pressione `F5` ou vá em Run > Start Debugging
3. Selecione ".NET Core Launch (web)"

### Frontend (VS Code)

1. Abra a pasta `apps/frontend` no VS Code
2. Instale a extensão "Debugger for Chrome"
3. Pressione `F5` ou vá em Run > Start Debugging

## 🧪 Qualidade de Código

### Backend

```bash
cd apps/backend

# Análise de código
dotnet format

# Code cleanup
dotnet build /p:WarningsAsErrors=true
```

### Frontend

```bash
cd apps/frontend

# Linting
npm run lint

# Formatação
npm run format

# Type check
npm run type-check
```

## 📝 Convenções

### Commits

Use Conventional Commits:

```
feat: adiciona nova funcionalidade
fix: corrige bug
docs: atualiza documentação
style: formatação, ponto e vírgula, etc
refactor: refatoração de código
test: adiciona ou corrige testes
chore: atualiza dependências, configs, etc
```

### Branches

- `main` - Produção
- `develop` - Desenvolvimento
- `feature/nome-da-feature` - Nova funcionalidade
- `fix/nome-do-bug` - Correção de bug
- `hotfix/nome-do-hotfix` - Correção urgente em produção

### Pull Requests

1. Crie uma branch a partir de `develop`
2. Faça suas alterações
3. Escreva testes
4. Atualize documentação se necessário
5. Abra PR para `develop`
6. Aguarde code review

## 🆘 Problemas Comuns

### Erro de conexão com banco de dados

```bash
# Verifique se o PostgreSQL está rodando
docker-compose ps

# Restart do container
docker-compose restart postgres

# Logs do banco
docker-compose logs postgres
```

### Portas em uso

```bash
# Verificar processo usando porta 5000
lsof -i :5000

# Matar processo
kill -9 <PID>
```

### Limpar builds

```bash
# Backend
cd apps/backend
dotnet clean
rm -rf */bin */obj

# Frontend
cd apps/frontend
rm -rf node_modules .next build dist
npm install
```

## 📚 Recursos

- [Documentação do .NET](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [PostgreSQL](https://www.postgresql.org/docs/)
- [React Docs](https://react.dev/) (quando aplicável)
- [TypeScript Docs](https://www.typescriptlang.org/docs/)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie sua feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'feat: Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📞 Suporte

- Abra uma issue no GitHub
- Entre em contato com o time de desenvolvimento
