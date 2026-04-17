# Quality Checklist - Task Manager

Este documento contém as diretrizes obrigatórias que devem ser seguidas antes de considerar qualquer tarefa como concluída.

## ✅ Checklist Obrigatório

### 1. Testes Unitários

#### Backend
- [ ] Executar testes: `cd apps/backend && dotnet test TaskManager.sln`
- [ ] Verificar que **todos os testes passam** (231+ testes)
- [ ] Cobertura adequada para novas funcionalidades
- [ ] Sem testes ignorados (skip) sem justificativa

#### Frontend
- [ ] Executar testes: `cd apps/frontend && npm run test -- --run`
- [ ] Verificar que **todos os testes passam** (65+ testes)
- [ ] Cobertura adequada para novos componentes
- [ ] Testes de integração para fluxos principais

### 2. Qualidade de Código

#### Backend - Warnings
- [ ] Build sem warnings: `cd apps/backend && dotnet build TaskManager.sln /warnaserror`
- [ ] **Zero warnings aceitos** no build de produção
- [ ] Corrigir todos os warnings de nullable
- [ ] Corrigir todos os warnings de API obsoletas

#### Frontend - Lint
- [ ] Executar lint: `cd apps/frontend && npm run lint`
- [ ] **Zero erros de lint**
- [ ] **Zero warnings não justificados**
- [ ] Seguir regras do ESLint configuradas
- [ ] Usar `_` prefix apenas quando variável não pode ser removida

### 3. Compilação

#### Backend
- [ ] Compilação limpa: `cd apps/backend && dotnet build TaskManager.sln`
- [ ] Nenhum erro de compilação
- [ ] Publicação funcional: `dotnet publish TaskManager.sln`

#### Frontend
- [ ] Build de produção: `cd apps/frontend && npm run build`
- [ ] Nenhum erro TypeScript
- [ ] Bundle gerado corretamente em `dist/`
- [ ] Tamanho dos chunks verificado

### 4. Padrões do Projeto

#### Backend
- [ ] Usar `BaseEntity` ou `BaseEntitySoft` para entidades
- [ ] Usar `BaseRepository<T>` com método `Query()` retornando `IQueryable`
- [ ] **Todas as mensagens** devem usar `IResourceStringLocalizer`
- [ ] Controllers devem usar `IRequestContext.UserId` para identificar usuário
- [ ] Usar `ReturnData` com factory methods ou object initializers
- [ ] Testes devem usar `FakeItEasy` (não Moq)
- [ ] Soft delete: usar propriedade `Deleted` (não `Active`)
- [ ] Migrations seguem padrão snake_case

#### Frontend
- [ ] Usar `verbatimModuleSyntax` com `import type` para tipos
- [ ] Props de componentes tipadas com TypeScript
- [ ] Hooks devem ter dependências corretas no array
- [ ] Usar axios para chamadas HTTP
- [ ] Seguir estrutura de pastas existente
- [ ] Testes com Vitest e Testing Library

### 5. Internacionalização (i18n)

#### Backend
- [ ] **NUNCA** retornar strings literais nas respostas da API
- [ ] Adicionar todas as chaves em `LocalizationDictionary.cs`
- [ ] Usar `_localizer.GetString("ChaveI18n")` em serviços
- [ ] Mensagens de erro devem ter chaves i18n
- [ ] Mensagens de sucesso devem ter chaves i18n

### 6. Segurança

- [ ] Senhas/chaves nunca em plain text
- [ ] Usar hashing apropriado (SHA-256, bcrypt, etc)
- [ ] Validar entrada de usuário
- [ ] Usar `[Authorize]` em controllers que precisam autenticação
- [ ] Chaves sensíveis mostradas apenas uma vez
- [ ] CORS configurado corretamente

### 7. CI/CD (Jenkins)

- [ ] Pipeline não deve falhar
- [ ] Todos os estágios passam:
  - Restore/Install
  - Build
  - Test
  - Lint (frontend)
  - Warnings check (backend)
- [ ] Docker build funcional (se aplicável)

### 8. Documentação

- [ ] README atualizado se necessário
- [ ] Comentários em código complexo
- [ ] API endpoints documentados
- [ ] Migrations criadas e nomeadas apropriadamente

### 9. Git

- [ ] Commit messages descritivas
- [ ] Sem arquivos desnecessários comitados
- [ ] `.gitignore` atualizado se necessário
- [ ] Branch apropriado para a feature

## 🔄 Fluxo de Verificação

Executar na ordem:

```bash
# 1. Backend - Build e Warnings
cd apps/backend
dotnet build TaskManager.sln /warnaserror

# 2. Backend - Testes
dotnet test TaskManager.sln

# 3. Frontend - Lint
cd ../frontend
npm run lint

# 4. Frontend - Build
npm run build

# 5. Frontend - Testes
npm run test -- --run
```

## ❌ Critérios de Bloqueio

Uma tarefa **NÃO pode ser concluída** se:

1. Qualquer teste falha (backend ou frontend)
2. Existem warnings no backend
3. Existem erros de lint no frontend
4. Build falha em qualquer projeto
5. Strings hardcoded sem i18n no backend
6. Padrões do projeto não são seguidos

## 📝 Notas Importantes

- **Sempre** execute o checklist completo antes de commitar
- **Nunca** ignore warnings ou erros "temporariamente"
- **Sempre** corrija problemas antes de adicionar novas features
- Use este documento como guia durante desenvolvimento
- Mantenha este checklist atualizado com novos padrões

## 🎯 Meta de Qualidade

- 0 warnings no backend
- 0 erros de lint no frontend
- 100% testes passando
- Build sempre limpo
- CI/CD sempre verde
