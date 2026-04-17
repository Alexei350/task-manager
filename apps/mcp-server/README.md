# Task Manager MCP Server

Servidor MCP (Model Context Protocol) que permite que IAs como Claude Desktop acessem seu Task Manager pessoal.

## 🔐 Autenticação

Este servidor MCP usa **autenticação direta com API Key** via header `X-Api-Key`.

**Como funciona:**
- O MCP Server envia sua API Key em cada requisição usando o header `X-Api-Key`
- O backend valida a chave e autentica automaticamente o usuário
- Não é necessário fazer login ou gerar tokens JWT

**Nota:** Se você estiver construindo outra aplicação (não MCP), pode usar o endpoint `POST /Authentication/ApiKeyLogin` para converter uma API Key em um token JWT.

## 📋 Pré-requisitos

- Node.js 18+ instalado
- Conta no Task Manager com API Key gerada

## 🚀 Instalação

### 1. Gerar API Key no Task Manager

1. Acesse https://alexei.dev.br (ou sua instância)
2. Faça login na sua conta
3. Vá em **Configurações** → **API Keys**
4. Clique em **"Gerar Chave"**
5. Dê um nome (ex: "Claude Desktop")
6. **Copie a chave gerada** (ex: `tm_sk_abc123...`)
   - ⚠️ **Importante:** Você só verá esta chave uma vez!

### 2. Clonar e Instalar o MCP

```bash
# Clone o repositório
git clone <seu-repositorio>
cd task-manager/apps/mcp-server

# Instale as dependências
npm install

# Compile o código TypeScript
npm run build
```

### 3. Configurar Claude Desktop

Edite o arquivo de configuração do Claude Desktop:

**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`  
**Windows:** `%APPDATA%\Claude\claude_desktop_config.json`  
**Linux:** `~/.config/Claude/claude_desktop_config.json`

Adicione a seguinte configuração:

```json
{
  "mcpServers": {
    "task-manager": {
      "command": "node",
      "args": ["/caminho/completo/para/task-manager/apps/mcp-server/build/index.js"],
      "env": {
        "API_URL": "https://alexei.dev.br",
        "API_KEY": "tm_sk_abc123..."
      }
    }
  }
}
```

**Atenção:**
- Substitua `/caminho/completo/para/...` pelo caminho absoluto no seu sistema
- Substitua `tm_sk_abc123...` pela sua API Key real
- Substitua `https://alexei.dev.br` pela URL da sua API se for diferente

### 4. Reiniciar Claude Desktop

Feche completamente o Claude Desktop e abra novamente para carregar a nova configuração.

## 🎯 Uso

Depois de configurado, você pode pedir ao Claude:

- "Liste minhas tarefas"
- "Crie uma tarefa para estudar Node.js com status pendente"
- "Mostre a página 2 das minhas tarefas"
- "Crie uma tarefa 'Reunião com cliente' para amanhã às 14h"
- "Atualize a tarefa X para status 'InProgress'"
- "Marque a tarefa Y como finalizada"
- "Delete a tarefa Z"
- "Mostre os detalhes da tarefa ABC-123-DEF"

## 🛠️ Ferramentas Disponíveis

### `list_tasks`
Lista suas tarefas com paginação.

**Parâmetros:**
- `page` (opcional): Número da página (padrão: 1)
- `pageSize` (opcional): Tarefas por página (padrão: 10)

**Exemplo:**
```
Liste minhas tarefas da página 2
```

### `get_task`
Obtém os detalhes de uma tarefa específica pelo ID.

**Parâmetros:**
- `id` (obrigatório): ID da tarefa no formato GUID

**Exemplo:**
```
Mostre os detalhes da tarefa abc-123-def-456
```

### `create_task`
Cria uma nova tarefa na sua lista pessoal.

**Parâmetros:**
- `status` (opcional): Status da tarefa (padrão: 1 - Pending)
  - 0 = Unknown
  - 1 = Pending
  - 2 = InProgress
  - 3 = Finished
  - 4 = Paused
  - 5 = Cancelled
- `description` (obrigatório): Descrição da tarefa
- `observation` (opcional): Observações adicionais sobre a tarefa
- `timeSpent` (opcional): Tempo gasto na tarefa no formato ISO duration (ex: PT1H30M para 1h30min)
- `dueDate` (opcional): Data de vencimento no formato ISO (ex: 2026-01-15T10:00:00Z)

**Exemplo:**
```
Crie uma tarefa "Estudar TypeScript" com status InProgress e prazo para sexta-feira
```

### `update_task`
Atualiza uma tarefa existente.

**Parâmetros:**
- `id` (obrigatório): ID da tarefa no formato GUID
- `status` (obrigatório): Novo status da tarefa (0-5)
- `description` (obrigatório): Nova descrição
- `observation` (opcional): Novas observações
- `timeSpent` (opcional): Tempo gasto atualizado (formato ISO duration)
- `dueDate` (opcional): Nova data de vencimento (formato ISO)

**Exemplo:**
```
Atualize a tarefa abc-123 para status Finished
```

### `delete_task`
Deleta uma tarefa pelo ID.

**Parâmetros:**
- `id` (obrigatório): ID da tarefa no formato GUID

**Exemplo:**
```
Delete a tarefa xyz-789
```

## 📊 Formato de Resposta

O MCP Server agora retorna respostas mais informativas e formatadas:

**Exemplo de resposta de sucesso:**
```
✓ Success

Messages:
  ✓ Success: Tarefa criada com sucesso

Data:
ID: abc-123-def-456
    Description: Estudar TypeScript
    Status: InProgress
    Created: 08/01/2026 15:30:00
    Due Date: 12/01/2026 18:00:00
```

**Exemplo de resposta de erro:**
```
✗ Failed

Messages:
  ✗ Error: Dados incompletos
```

**Exemplo de lista paginada:**
```
✓ Success

Data:
  Total items: 3
  [1] ID: abc-123
    Description: Tarefa 1
    Status: Pending
    Created: 08/01/2026 10:00:00
  [2] ID: def-456
    Description: Tarefa 2
    Status: InProgress
    Created: 07/01/2026 14:30:00
  [3] ID: ghi-789
    Description: Tarefa 3
    Status: Finished
    Created: 06/01/2026 09:15:00
    Completed: 06/01/2026 17:45:00

Pagination:
  Page: 1 of 2
  Total items: 15
  Page size: 10
```

## 🔒 Segurança

- ✅ Cada usuário tem sua própria API Key
- ✅ API Key fica armazenada apenas localmente no seu computador
- ✅ Servidor MCP roda localmente, não expõe dados
- ✅ Todas as requisições são autenticadas via API Key
- ✅ Você só acessa suas próprias tarefas

## 🐛 Troubleshooting

### Erro: "API_KEY environment variable is required!"
- Verifique se você adicionou `API_KEY` na configuração do Claude Desktop
- Confirme que a chave está entre aspas no JSON

### Erro: "Invalid API key"
- Sua API Key pode ter sido revogada
- Gere uma nova chave no Task Manager

### Claude não reconhece o MCP
- Certifique-se de reiniciar completamente o Claude Desktop
- Verifique se o caminho para `index.js` está correto e é absoluto
- Verifique os logs em: `~/Library/Logs/Claude/mcp*.log` (macOS)

### Erro de compilação
```bash
# Limpe e recompile
npm run clean
npm install
npm run build
```

## 📝 Scripts Disponíveis

- `npm run build` - Compila o TypeScript
- `npm run dev` - Modo desenvolvimento com watch
- `npm run clean` - Limpa os arquivos compilados

## 🔄 Atualizações

Para atualizar o MCP server:

```bash
cd task-manager/apps/mcp-server
git pull
npm install
npm run build
```

Depois reinicie o Claude Desktop.

## 📚 Recursos

- [Model Context Protocol](https://modelcontextprotocol.io/)
- [Claude Desktop](https://claude.ai/download)
- [Task Manager API Docs](https://alexei.dev.br/swagger)
