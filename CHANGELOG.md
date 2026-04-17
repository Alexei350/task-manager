# Changelog

Todas as mudanças notáveis do projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Em Desenvolvimento
- Pacotes compartilhados de tipos TypeScript
- Biblioteca de componentes UI

## [0.3.0] - 2025-12-11

### Added - Frontend UX Improvements
- ✨ Visualização Kanban Board interativa para tarefas
- ✨ Modal dialogs para criar e editar tarefas (melhor UX)
- ✨ Drag-and-drop para alternar status das tarefas
- ✨ Toggle entre visualização Kanban e Tabela
- ✨ Alteração rápida de status via dropdown na visualização de tabela
- ✨ Estados de loading aprimorados e feedback visual
- ✨ Design responsivo para mobile
- ✨ Confirmação antes de excluir tarefas
- ✨ Empty states informativos
- 🧪 Testes unitários para KanbanBoard e Modal (15 testes no total)
- 📊 Cobertura de testes frontend: 73.65%
- 📘 Documentação atualizada do frontend com novas funcionalidades

### Changed
- 🔄 Removido formulário inline feio de criar tarefa
- 🔄 Interface atualizada com melhor hierarquia visual
- 🔄 Aumentado PAGE_SIZE de 10 para 100 para melhor aproveitamento do Kanban
- 🎨 CSS aprimorado com novos estilos para Kanban, Modal e componentes

### Fixed
- 🐛 Corrigido import faltante de useContext em AuthContext
- 🐛 Corrigidos testes unitários para refletir novas mudanças de UI

### Technical
- ⚡ Componente KanbanBoard com 5 colunas de status
- ⚡ Componente Modal reutilizável com gerenciamento de diálogo nativo
- ⚡ Melhorias de acessibilidade (ARIA labels)
- ⚡ Seguindo melhores práticas de UX e design patterns

## [0.2.0] - 2025-12-11

### Added - Reestruturação para Monorepo
- ✨ Estrutura de monorepo com separação de apps
- 📁 Pasta `apps/backend/` com toda aplicação .NET existente
- 📁 Pasta `apps/frontend/` preparada para futuro desenvolvimento
- 📁 Pasta `packages/` para código compartilhado
- 📁 Pasta `docs/` com documentação centralizada
- 📄 `task-manager.code-workspace` - Workspace multi-folder do VS Code
- 📘 Guia de desenvolvimento completo
- 📘 Documentação da estrutura do projeto
- 📘 ADR sobre decisão de usar monorepo
- 📘 Documentação do CI/CD
- 🚀 Quick Start Guide

### Changed
- 🔄 Movida toda estrutura `src/` para `apps/backend/`
- 🔄 Jenkinsfile mantido em `apps/backend/` para simplicidade
- 🔄 Atualizados scripts de deploy e migrations
- 🔄 README principal atualizado para refletir estrutura monorepo

### Technical
- 🧪 95 testes unitários funcionando na nova estrutura
- 📊 Cobertura de código: 36.2%
- ✅ Pipeline Jenkins focado no backend (frontend terá seu próprio quando implementado)

## [0.1.0] - 2025-12-09

### Added - Melhorias de Testes
- ✅ Testes para Repository (TaskRepository, UserRepository)
- ✅ Testes para Utils (StringExtensions, ValidationExtensions)
- 🔧 Correções em métodos de validação para tratar null
- 📊 Cobertura aumentada de 27.9% para 36.2%

### Changed
- 🐛 Corrigidos valores de teste de CPF e CNPJ para usar números válidos
- 🐛 Corrigidos testes de UserRepository para soft delete
- 🔄 Resolvido conflito de namespace com Task entity

## [0.0.1] - Versão Inicial

### Added - Projeto Base
- 🎯 API RESTful em .NET 10
- 🗄️ PostgreSQL 17 com Entity Framework Core
- 🔐 Autenticação JWT
- 🔐 Autenticação com Google OAuth
- 👤 CRUD de Usuários
- ✅ CRUD de Tarefas
- 🧪 49 testes unitários iniciais
- 🐳 Docker e Docker Compose
- 🔄 CI/CD com Jenkins
- 📊 Code coverage com Cobertura
- 📝 Swagger/OpenAPI documentation
- 🌍 Internacionalização (i18n)
- 🔒 Soft delete para usuários

### Endpoints
- `POST /api/authentication/login` - Login
- `POST /api/authentication/refresh-token` - Refresh token
- `POST /api/authentication/google-login` - Google OAuth
- `GET /api/me` - Usuário autenticado
- `PUT /api/me` - Atualizar perfil
- `GET /api/tasks` - Listar tarefas
- `POST /api/tasks` - Criar tarefa
- `PUT /api/tasks/{id}` - Atualizar tarefa
- `DELETE /api/tasks/{id}` - Deletar tarefa
- `GET /api/users` - Listar usuários (Admin)
- `POST /api/users` - Criar usuário (Admin)
- `PUT /api/users/{id}` - Atualizar usuário (Admin)
- `DELETE /api/users/{id}` - Deletar usuário (Admin)

---

## Tipos de Mudanças

- `Added` - para novas funcionalidades
- `Changed` - para mudanças em funcionalidades existentes
- `Deprecated` - para funcionalidades que serão removidas
- `Removed` - para funcionalidades removidas
- `Fixed` - para correções de bugs
- `Security` - em caso de vulnerabilidades
- `Technical` - para mudanças técnicas que não afetam usuários
