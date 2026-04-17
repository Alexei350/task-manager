# Docker Compose - Configuração Completa

Este projeto possui dois arquivos Docker Compose para máxima flexibilidade:

## Arquivos

- `docker-compose.db.yaml`: Contém apenas o banco de dados PostgreSQL
- `docker-compose.yaml`: Contém a aplicação completa (API + Frontend)

## Como usar

### 1. Configuração inicial

Copie o arquivo de exemplo de variáveis de ambiente:
```bash
cp .env.example .env
```

Edite o arquivo `.env` com suas configurações:
```bash
nano .env
```

### 2. Executar apenas o banco de dados

```bash
docker-compose -f docker-compose.db.yaml up -d
```

### 3. Executar a aplicação completa (API + Frontend)

```bash
docker-compose up -d
```

### 4. Executar tudo junto (recomendado)

Para executar tanto o banco quanto a aplicação completa:
```bash
# Primeiro, criar a rede compartilhada (apenas uma vez)
docker network create task-manager-network

# Subir o banco de dados
docker-compose -f docker-compose.db.yaml up -d

# Subir a aplicação (API + Frontend)
docker-compose up -d
```

### 5. Parar os serviços

Para parar apenas a aplicação:
```bash
docker-compose down
```

Para parar apenas o banco:
```bash
docker-compose -f docker-compose.db.yaml down
```

Para parar tudo:
```bash
docker-compose down
docker-compose -f docker-compose.db.yaml down
```

## Serviços disponíveis

### Backend (API)
- **Porta**: 30000
- **URL**: http://localhost:30000
- **Tecnologia**: .NET Core
- **Container**: task-manager-api

### Frontend
- **Porta**: 3000
- **URL**: http://localhost:3000
- **Tecnologia**: React + Vite
- **Container**: task-manager-frontend

### Banco de dados
- **Porta**: 5432
- **Tecnologia**: PostgreSQL 17
- **Container**: task-manager-database

## Vantagens desta separação

1. **Flexibilidade**: Você pode executar apenas o banco ou a aplicação completa
2. **Desenvolvimento**: Útil quando você quer rodar partes do sistema localmente
3. **Ambientes diferentes**: Permite diferentes configurações para diferentes ambientes
4. **Debugging**: Facilita o debug isolando componentes
5. **Escalabilidade**: Cada serviço pode ser escalado independentemente

## Configurações de rede

- Banco de dados: `5432`
- API Backend: `30000`
- Frontend: `3000`
- Todos usam a rede `task-manager-network` para comunicação interna

## Conectar-se ao banco externamente

Se você quiser conectar ao banco de dados de fora dos containers:
- Host: `localhost`
- Porta: `5432`
- Database: `task_manager`
- User/Password: Conforme definido no arquivo `.env`

## Variáveis de ambiente

As seguintes variáveis podem ser configuradas no arquivo `.env`:

- `POSTGRES_USER`: Usuário do banco de dados
- `POSTGRES_PASSWORD`: Senha do banco de dados
- `DB_HOST`: Host do banco (padrão: task-manager-database)
- `VITE_API_BASE_URL`: URL base da API para o frontend