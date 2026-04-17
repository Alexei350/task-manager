# Task Manager API

[![Build Status](https://jenkins.alexei.dev.br/job/task-manager-backend/job/main/badge/icon)](https://jenkins.alexei.dev.br/job/task-manager-backend/job/main/)

Sistema de gerenciamento de tarefas desenvolvido em .NET 10 com PostgreSQL.

## 🚀 Tecnologias

- **.NET 10** - Framework principal
- **PostgreSQL 17** - Banco de dados
- **Entity Framework Core** - ORM
- **JWT Authentication** - Autenticação e autorização
- **Google OAuth** - Login social
- **xUnit** - Testes unitários
- **Docker** - Containerização
- **Jenkins** - CI/CD
- **Swagger** - Documentação da API

## 📋 Funcionalidades

- ✅ Gerenciamento completo de tarefas (CRUD)
- ✅ Sistema de autenticação com JWT
- ✅ Login com Google
- ✅ Gerenciamento de usuários
- ✅ Perfil de usuário (Me)
- ✅ Internacionalização (i18n) - PT, EN, ES
- ✅ Soft delete
- ✅ 49 testes unitários

## 🏗️ Arquitetura

```
src/
├── TaskManager/              # Projeto principal da API
│   ├── Controllers/          # Endpoints da API
│   ├── Service/              # Lógica de negócio
│   ├── Repository/           # Acesso a dados
│   ├── Models/               # DTOs e entidades
│   │   ├── Entities/         # Entidades do banco
│   │   ├── Request/          # Models de requisição
│   │   └── Return/           # Models de resposta
│   ├── Context/              # Contexto do EF Core
│   ├── Migrations/           # Migrations do banco
│   └── Utils/                # Utilitários
└── TaskManager.UnitTests/    # Testes unitários
```

## 🔧 Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL 17](https://www.postgresql.org/) (se não usar Docker)

## 🚀 Como executar

### Opção 1: Com Docker Compose (Recomendado)

1. Clone o repositório:
```bash
git clone https://github.com/Alexei350/task-manager.git
cd task-manager
```

2. Configure as variáveis de ambiente:
```bash
# Crie um arquivo .env na raiz do projeto
echo "POSTGRES_USER=seu_usuario" > .env
echo "POSTGRES_PASSWORD=sua_senha" >> .env
```

3. Inicie os containers:
```bash
docker-compose up -d
```

4. Acesse a API:
- **API**: http://localhost:30000
- **Swagger**: http://localhost:30000/swagger

### Opção 2: Desenvolvimento Local

1. Configure o banco de dados no `appsettings.json` ou use variáveis de ambiente:
```bash
export DB_HOST=localhost
export DB_NAME=task_manager
export POSTGRES_USER=seu_usuario
export POSTGRES_PASSWORD=sua_senha
```

2. Restaure as dependências:
```bash
cd src
dotnet restore
```

3. Execute as migrations:
```bash
dotnet ef database update --project TaskManager
```

4. Execute a aplicação:
```bash
cd TaskManager
dotnet run
```

5. Acesse a API em http://localhost:8080

## 🧪 Executar Testes

```bash
cd src
dotnet test TaskManager.UnitTests/TaskManager.UnitTests.csproj
```

Para gerar relatório de cobertura:
```bash
dotnet test TaskManager.UnitTests/TaskManager.UnitTests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./TestResults
```

## 📦 Build e Deploy

O projeto utiliza Jenkins para CI/CD automatizado:

1. **Testes**: Executa todos os testes unitários
2. **Build**: Constrói a imagem Docker
3. **Push**: Envia para o Docker Hub
4. **Deploy**: Faz deploy automático no servidor

### Build manual da imagem Docker:

```bash
cd src
docker build -t alexei350/task-manager:latest .
```

## 🔑 Configuração

### Variáveis de Ambiente

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `DB_HOST` | Host do PostgreSQL | localhost |
| `DB_NAME` | Nome do banco de dados | task_manager |
| `POSTGRES_USER` | Usuário do banco | - |
| `POSTGRES_PASSWORD` | Senha do banco | - |
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | Development |

### JWT Configuration

Edite o `appsettings.json` para configurar o JWT:

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-aqui",
    "Issuer": "seu-issuer",
    "Audience": "sua-audience"
  }
}
```

### Google OAuth

Configure o `GoogleClientId` no `appsettings.json` com suas credenciais do Google Cloud Console.

## 📚 Endpoints Principais

### Autenticação
- `POST /Authentication/login` - Login com email/senha
- `POST /Authentication/google-login` - Login com Google

### Tarefas
- `GET /Task` - Listar tarefas
- `POST /Task` - Criar tarefa
- `PUT /Task/{id}` - Atualizar tarefa
- `DELETE /Task/{id}` - Remover tarefa

### Usuários
- `GET /User` - Listar usuários
- `POST /User` - Criar usuário
- `PUT /User/{id}` - Atualizar usuário
- `DELETE /User/{id}` - Remover usuário

### Perfil
- `GET /Me` - Obter dados do usuário autenticado
- `PUT /Me` - Atualizar dados do usuário autenticado

## 🗄️ Banco de Dados

### Executar Migrations

```bash
cd src/TaskManager
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

### Scripts de Migração

Os scripts estão disponíveis em:
- `scripts/migrate.sh` - Migração local
- `scripts/migrate-server.sh` - Migração no servidor

## 🐳 Docker

### Comandos úteis

```bash
# Ver logs da aplicação
docker logs -f task-manager-api

# Ver logs do banco
docker logs -f task-manager-database

# Parar containers
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Reconstruir imagens
docker-compose up -d --build
```

## 📄 Licença

Este projeto está sob a licença MIT.

## 👤 Autor

**Alexei350**

- GitHub: [@Alexei350](https://github.com/Alexei350)

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

⭐ Se este projeto foi útil para você, considere dar uma estrela!
