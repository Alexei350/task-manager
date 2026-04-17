# CI/CD Pipeline

Documentação do pipeline de integração e entrega contínua do Task Manager.

## 🔄 Visão Geral

Cada aplicação tem seu próprio Jenkinsfile para manter a configuração simples e fácil de manter.

## 📍 Localização

```
task-manager/
├── apps/
│   ├── backend/
│   │   └── Jenkinsfile       # Pipeline do backend
│   └── frontend/
│       └── Jenkinsfile       # Pipeline do frontend
```

## 🎯 Estrutura

- **Backend**: Pipeline completo com testes, build, Docker e deploy
- **Frontend**: Pipeline dedicado (npm ci, testes, build e imagem Docker)

## 🔧 Stages do Backend

### 1. Backend: Tests
```bash
cd apps/backend
dotnet restore
dotnet test --collect:"XPlat Code Coverage"
```

**Outputs:**
- Resultados de testes (JUnit XML)
- Cobertura de código (Cobertura XML)
- Artefatos arquivados

### 2. Backend: Build Docker
```bash
cd apps/backend
docker build -t alexei350/task-manager:${BUILD_NUMBER}
docker tag alexei350/task-manager:${BUILD_NUMBER} alexei350/task-manager:latest
```

### 3. Backend: Push Docker
```bash
docker push alexei350/task-manager:latest
docker push alexei350/task-manager:${BUILD_NUMBER}
```

### 4. Backend: Deploy
```bash
ssh alexei@alexei.dev.br '
  docker compose pull task-manager-api
  docker compose up -d --no-deps task-manager-api
'
```

## 🎨 Stages do Frontend

### 1. Frontend: Install & Test
```bash
cd apps/frontend
npm ci
npm run test:ci
```

### 2. Frontend: Build
```bash
cd apps/frontend
npm run build
```

### 3. Frontend: Docker
```bash
cd apps/frontend
docker build -t alexei350/task-manager-frontend:${BUILD_NUMBER} .
docker tag alexei350/task-manager-frontend:${BUILD_NUMBER} alexei350/task-manager-frontend:latest
docker push alexei350/task-manager-frontend:${BUILD_NUMBER}
docker push alexei350/task-manager-frontend:latest
```

## ⚙️ Configuração Jenkins

### Credentials Necessárias

1. **docker-hub** (Username/Password)
   - User: Seu usuário Docker Hub
   - Password: Token de acesso

2. **server-ssh** (SSH Key)
   - Chave privada SSH para deploy no servidor

### Webhooks

Configure webhook no GitHub/GitLab para o backend:
```
URL: https://jenkins.alexei.dev.br/job/task-manager-backend/
Events: Push, Pull Request
Branches: main
```

Quando criar o frontend, configure outro webhook:
```
URL: https://jenkins.alexei.dev.br/job/task-manager-frontend/
Events: Push, Pull Request
Branches: main
```

## 📊 Relatórios Gerados

### Backend
- ✅ **Test Results**: JUnit XML
- 📊 **Code Coverage**: Cobertura report com trend
- 📦 **Artifacts**: Resultados de teste arquivados

### Frontend
- ✅ **Testes**: Vitest (coverage v8)
- 📦 **Artefatos**: `coverage/` do Vitest e build do Vite

## 🚀 Executando Manualmente

### Via Jenkins UI
1. Acesse: https://jenkins.alexei.dev.br
2. Selecione o job "task-manager-backend"
3. Clique em "Build Now"

### Via API
```bash
curl -X POST https://jenkins.alexei.dev.br/job/task-manager-backend/build \
  --user $JENKINS_USER:$JENKINS_TOKEN
```

## 🔍 Logs e Debugging

### Ver logs de um build
```bash
# No Jenkins
Build #123 → Console Output

# Via CLI
jenkins-cli get-build task-manager 123
```

### Logs do Docker
```bash
# No servidor
docker logs task-manager-api

# Via Jenkins (stage de deploy)
ssh alexei@alexei.dev.br 'docker logs --tail 50 task-manager-api'
```

## 🎛️ Variáveis de Ambiente

```groovy
DOCKER_IMAGE  = 'alexei350/task-manager'
DOCKER_REGISTRY = 'docker.io'
```

## 🔐 Segurança

### Secrets
- ❌ Nunca commitar credenciais
- ✅ Usar Jenkins Credentials Store
- ✅ Variáveis sensíveis com `withCredentials`

### Docker
```groovy
withCredentials([usernamePassword(...)]) {
  sh 'echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin'
  // ... push images
  sh 'docker logout'  // Sempre fazer logout
}
```

## 📈 Otimizações

### Cache de Dependências
```groovy
// Backend (.NET)
- Restaurar apenas se .csproj mudou
- Usar camadas Docker eficientemente

// Frontend (Node)
- npm ci em vez de npm install
- Cache de node_modules entre builds
```

### Build Paralelo
```groovy
parallel {
  stage('Backend') { /* ... */ }
  stage('Frontend') { /* ... */ }
}
```

## 🐛 Troubleshooting

### Build falha mas código está correto
```bash
# Limpar workspace
rm -rf workspace/task-manager

# Rebuild do zero
Build → Delete Workspace → Build Now
```

### Docker push falha
```bash
# Verificar credenciais
Jenkins → Credentials → docker-hub → Test Connection

# Verificar rate limit
docker pull alpine  # Se falhar, rate limited
```

### Deploy SSH falha
```bash
# Testar conexão
ssh -i ~/.ssh/id_rsa alexei@alexei.dev.br 'echo "OK"'

# Verificar chave no Jenkins
Jenkins → Credentials → server-ssh → Verify
```

## 📝 Próximos Passos

- Criar job no Jenkins apontando para `apps/frontend/Jenkinsfile` (ex: `task-manager-frontend`)
- Configurar webhook dedicado para o pipeline do frontend

## 🔗 Referências

- [Jenkins Pipeline Syntax](https://www.jenkins.io/doc/book/pipeline/syntax/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Git Commit Patterns](https://www.conventionalcommits.org/)
