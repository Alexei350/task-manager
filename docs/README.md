# Documentação

Documentação técnica e guias do projeto Task Manager.

## 📚 Conteúdo

Esta pasta contém:
- Diagramas de arquitetura
- Guias de desenvolvimento
- Documentação de APIs
- Decisões técnicas (ADRs)
- Guias de deploy
- Troubleshooting

## 🗂️ Estrutura Planejada

```
docs/
├── architecture/          # Diagramas e descrições da arquitetura
│   ├── backend.md
│   ├── frontend.md
│   └── database.md
├── api/                   # Documentação da API
│   ├── authentication.md
│   ├── tasks.md
│   └── users.md
├── guides/                # Guias práticos
│   ├── development.md
│   ├── testing.md
│   └── deployment.md
├── decisions/             # ADRs (Architecture Decision Records)
│   └── 001-monorepo-structure.md
└── troubleshooting/       # Resolução de problemas comuns
    └── common-issues.md
```

## 🚀 Como Começar

1. Leia o [README principal](../README.md)
2. Configure o ambiente seguindo o [Guia de Desenvolvimento](./guides/development.md) (quando disponível)
3. Consulte a [Documentação da API](./api/) para integração

## 📝 Contribuindo com a Documentação

- Use Markdown para todos os documentos
- Mantenha a linguagem clara e objetiva
- Inclua exemplos práticos quando possível
- Atualize diagramas quando houver mudanças na arquitetura
- Use Mermaid para diagramas quando possível

## 🔗 Links Úteis

- [Backend](../apps/backend/README.md)
- [Frontend](../apps/frontend/README.md)
- [Pacotes Compartilhados](../packages/README.md)
