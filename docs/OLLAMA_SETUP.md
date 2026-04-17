# Configuração Ollama + Open WebUI com MCP Server

Este guia mostra como configurar um ambiente local completo com Ollama e Open WebUI para usar seu servidor MCP sem depender do Claude Desktop.

## 🎯 Visão Geral

- **Ollama**: Roda modelos de IA localmente (Llama, Mistral, etc.)
- **Open WebUI**: Interface web moderna para conversar com os modelos
- **MCP Server**: Seu servidor de tarefas integrado

## 📋 Pré-requisitos

- Docker e Docker Compose instalados
- Node.js 18+ (para compilar o MCP server)
- Pelo menos 8GB de RAM (16GB recomendado)
- 10-20GB de espaço em disco para os modelos

## 🚀 Instalação Rápida

### 1. Compilar o MCP Server

```bash
cd apps/mcp-server
npm install
npm run build
```

### 2. Configurar Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto com suas credenciais:

```bash
# API do Task Manager
API_URL=https://alexei.dev.br
API_KEY=tm_sk_sua_chave_aqui
```

### 3. Executar o Script de Setup

```bash
chmod +x scripts/setup-ollama.sh
./scripts/setup-ollama.sh
```

O script vai:
- ✅ Verificar dependências
- 📦 Subir containers do Ollama e Open WebUI
- 📥 Permitir escolher e baixar um modelo de IA
- 🎉 Configurar tudo automaticamente

## 🔧 Configuração Manual (Alternativa)

### Passo 1: Subir os Containers

```bash
docker compose -f docker-compose.ollama.yaml up -d
```

### Passo 2: Baixar um Modelo

```bash
# Modelo rápido e leve (2GB)
docker exec task-manager-ollama ollama pull llama3.2

# Ou Mistral (4GB, melhor qualidade)
docker exec task-manager-ollama ollama pull mistral

# Ou especializado em código (4GB)
docker exec task-manager-ollama ollama pull qwen2.5-coder
```

### Passo 3: Verificar Modelos Instalados

```bash
docker exec task-manager-ollama ollama list
```

## 🌐 Acessar a Interface

1. Abra http://localhost:3001 no navegador
2. No primeiro acesso, crie uma conta (será o administrador)
3. Comece a conversar com os modelos!

## 🔌 Configurar o MCP Server no Open WebUI

### Opção 1: Via Interface (Mais Fácil)

1. Acesse http://localhost:3001
2. Clique no ícone de **configurações** (⚙️)
3. Vá em **Admin Panel** → **Settings** → **Connections**
4. Role até **MCP Servers**
5. Clique em **+ Add MCP Server**

Configure assim:

```json
{
  "task-manager": {
    "command": "node",
    "args": [
      "/caminho/absoluto/para/task-manager/apps/mcp-server/dist/index.js"
    ],
    "env": {
      "API_URL": "https://alexei.dev.br",
      "API_KEY": "tm_sk_sua_chave_aqui"
    }
  }
}
```

**⚠️ Importante:** 
- Substitua `/caminho/absoluto/para/` pelo caminho real no seu sistema
- No Windows com WSL, use: `/mnt/d/Projetos/alexei350/task-manager/apps/mcp-server/dist/index.js`
- Substitua `tm_sk_sua_chave_aqui` pela sua API Key real

### Opção 2: Via Arquivo de Configuração

Se preferir editar diretamente, o Open WebUI armazena as configurações em:

```bash
# Dentro do container
docker exec -it task-manager-webui bash
# Editar /app/backend/data/config.json
```

### Opção 3: Via Docker (Recomendado para WSL)

Atualize o `docker-compose.ollama.yaml` para montar o MCP server:

```yaml
services:
  open-webui:
    # ... configurações existentes ...
    volumes:
      - open_webui_data:/app/backend/data
      - /mnt/d/Projetos/alexei350/task-manager/apps/mcp-server:/mcp-server:ro
    environment:
      # ... variáveis existentes ...
      - MCP_SERVER_PATH=/mcp-server/dist/index.js
      - API_URL=https://alexei.dev.br
      - API_KEY=tm_sk_sua_chave_aqui
```

## 🎨 Usando o MCP Server

Depois de configurado, você pode fazer perguntas como:

- "Liste minhas tarefas pendentes"
- "Crie uma tarefa para estudar Docker"
- "Mostre a página 2 das tarefas"
- "Adicione uma tarefa 'Reunião' para amanhã às 14h"

O modelo de IA vai automaticamente chamar as funções do seu MCP server!

## 🔍 Modelos Recomendados

| Modelo | Tamanho | Uso | Qualidade |
|--------|---------|-----|-----------|
| **llama3.2** | ~2GB | Tarefas gerais, rápido | ⭐⭐⭐ |
| **mistral** | ~4GB | Melhor equilíbrio | ⭐⭐⭐⭐ |
| **llama3.1** | ~5GB | Alta qualidade | ⭐⭐⭐⭐⭐ |
| **qwen2.5-coder** | ~4GB | Código/programação | ⭐⭐⭐⭐ |
| **phi3** | ~2GB | Compacto, Microsoft | ⭐⭐⭐ |

### Baixar Modelos Adicionais

```bash
# Dentro do container
docker exec task-manager-ollama ollama pull <nome-do-modelo>

# Ou via API
curl http://localhost:11434/api/pull -d '{"name":"mistral"}'
```

### Remover Modelos

```bash
docker exec task-manager-ollama ollama rm <nome-do-modelo>
```

## 🛠️ Comandos Úteis

### Gerenciar Containers

```bash
# Ver logs
docker compose -f docker-compose.ollama.yaml logs -f

# Ver logs de um serviço específico
docker compose -f docker-compose.ollama.yaml logs -f ollama
docker compose -f docker-compose.ollama.yaml logs -f open-webui

# Parar containers
docker compose -f docker-compose.ollama.yaml down

# Parar e remover volumes (CUIDADO: apaga dados)
docker compose -f docker-compose.ollama.yaml down -v

# Reiniciar containers
docker compose -f docker-compose.ollama.yaml restart

# Ver status
docker compose -f docker-compose.ollama.yaml ps
```

### Gerenciar Modelos

```bash
# Listar modelos instalados
docker exec task-manager-ollama ollama list

# Baixar novo modelo
docker exec task-manager-ollama ollama pull llama3.2

# Remover modelo
docker exec task-manager-ollama ollama rm llama3.2

# Informações sobre um modelo
docker exec task-manager-ollama ollama show llama3.2
```

### Testar API do Ollama

```bash
# Listar modelos via API
curl http://localhost:11434/api/tags

# Gerar texto
curl http://localhost:11434/api/generate -d '{
  "model": "llama3.2",
  "prompt": "Por que o céu é azul?",
  "stream": false
}'
```

## 🐛 Troubleshooting

### Container do Ollama não inicia

**Problema:** `Error starting userland proxy`

**Solução:** A porta 11434 já está em uso. Mude no `docker-compose.ollama.yaml`:

```yaml
ports:
  - "11435:11434"  # Use outra porta
```

### Open WebUI não conecta ao Ollama

**Problema:** `Connection refused`

**Solução:** Verifique se o container do Ollama está rodando:

```bash
docker ps | grep ollama
docker logs task-manager-ollama
```

### Modelo demora muito para responder

**Problema:** Respostas lentas

**Soluções:**
1. Use um modelo menor (`llama3.2` em vez de `llama3.1`)
2. Verifique uso de CPU/RAM: `docker stats`
3. Se tiver GPU NVIDIA, descomente a seção GPU no docker-compose

### MCP Server não funciona

**Problema:** Funções do MCP não são chamadas

**Verificações:**
1. MCP server está compilado: `ls -la apps/mcp-server/dist/index.js`
2. API Key está correta no `.env` ou configuração
3. Node.js está acessível no container
4. Verifique logs do Open WebUI: `docker logs task-manager-webui`

### Erro "spawn node ENOENT" no Open WebUI

**Problema:** Node.js não encontrado

**Solução:** Instale Node.js no container ou use uma imagem customizada:

```dockerfile
FROM ghcr.io/open-webui/open-webui:main

USER root
RUN apt-get update && apt-get install -y nodejs npm
USER 1000
```

## 🔐 Segurança

### Alterar Secret Key

No `docker-compose.ollama.yaml`, mude:

```yaml
environment:
  - WEBUI_SECRET_KEY=mude-para-algo-seguro-e-aleatorio
```

### Expor Publicamente (Cuidado!)

Se quiser acessar de outros dispositivos:

```yaml
ports:
  - "0.0.0.0:3001:8080"  # Permite acesso externo
```

**⚠️ Atenção:** Sempre use HTTPS e autenticação forte em produção!

## 📊 Recursos do Sistema

### Requisitos Mínimos

- **CPU:** 4 cores
- **RAM:** 8GB (16GB recomendado)
- **Disco:** 20GB livres
- **GPU:** Opcional (acelera muito com NVIDIA)

### Uso com GPU NVIDIA

Descomente no `docker-compose.ollama.yaml`:

```yaml
ollama:
  deploy:
    resources:
      reservations:
        devices:
          - driver: nvidia
            count: 1
            capabilities: [gpu]
```

Instale o NVIDIA Container Toolkit:

```bash
# Ubuntu/Debian
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -s -L https://nvidia.github.io/nvidia-docker/gpgkey | sudo apt-key add -
curl -s -L https://nvidia.github.io/nvidia-docker/$distribution/nvidia-docker.list | \
  sudo tee /etc/apt/sources.list.d/nvidia-docker.list

sudo apt-get update && sudo apt-get install -y nvidia-container-toolkit
sudo systemctl restart docker
```

## 🔗 Links Úteis

- [Ollama](https://ollama.ai/) - Site oficial
- [Open WebUI](https://github.com/open-webui/open-webui) - GitHub
- [Lista de Modelos](https://ollama.ai/library) - Todos os modelos disponíveis
- [MCP Protocol](https://modelcontextprotocol.io/) - Documentação do MCP

## 💡 Dicas

1. **Escolha o modelo certo:** Modelos maiores são melhores, mas mais lentos
2. **Use GPU se possível:** Acelera 10-20x
3. **Monitore recursos:** `docker stats` mostra uso em tempo real
4. **Faça backup:** Os volumes Docker guardam suas conversas e configurações
5. **Atualize regularmente:** `docker compose pull` pega versões novas

## 🎉 Pronto!

Agora você tem um ambiente completo de IA local, sem limites de mensagens e totalmente gratuito! 🚀

Para suporte, abra uma issue no GitHub ou consulte a documentação do [Open WebUI](https://docs.openwebui.com/).
