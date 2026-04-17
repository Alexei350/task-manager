# Decisão de Arquitetura: Estrutura Monorepo

**Status:** Aceito  
**Data:** 2025-12-11  
**Decisores:** Alexei Secretti

## Contexto

O projeto Task Manager inicialmente era composto apenas por uma API backend em .NET. Com a necessidade de adicionar uma interface front-end, surgiu a questão de como organizar o projeto.

## Decisão

Adotar uma estrutura de monorepo com as seguintes características:

```
task-manager/
├── apps/
│   ├── backend/           # API .NET existente
│   └── frontend/          # Nova aplicação front-end
├── packages/              # Código compartilhado
├── docs/                  # Documentação centralizada
└── README.md
```

## Justificativa

### Vantagens

1. **Código Compartilhado:** Facilita compartilhamento de tipos, utilitários e componentes
2. **Versionamento Único:** Todas as aplicações na mesma versão, reduzindo problemas de compatibilidade
3. **CI/CD Simplificado:** Um único pipeline pode gerenciar deploy de front e back
4. **Refatorações Seguras:** Mudanças podem ser testadas em todas as aplicações simultaneamente
5. **Onboarding Simplificado:** Novos desenvolvedores clonam apenas um repositório
6. **Sincronização de Tipos:** Tipos do backend podem ser exportados para o frontend

### Desvantagens e Mitigações

1. **Tamanho do Repositório:** Mitigado com .gitignore adequado e Git LFS se necessário
2. **Complexidade de Build:** Cada app mantém seu próprio sistema de build independente
3. **Tempo de Clone:** Aceitável para projetos de médio porte como este

## Alternativas Consideradas

### 1. Multi-repo (Repositórios Separados)
- ❌ Dificulta sincronização de tipos
- ❌ Requer múltiplos clones para desenvolver features completas
- ❌ Versionamento independente pode causar incompatibilidades

### 2. Monolito (Tudo em um projeto)
- ❌ Backend e frontend muito acoplados
- ❌ Deploy necessariamente simultâneo
- ❌ Tecnologias diferentes (.NET + JS) não se encaixam bem

## Consequências

### Positivas
- Melhor experiência de desenvolvimento
- Facilita refatorações cross-app
- Documentação centralizada
- Setup mais simples para novos desenvolvedores

### Negativas
- Necessidade de organizar bem a estrutura de pastas
- CI/CD precisa detectar mudanças específicas em cada app
- Possível aumento no tempo de CI se não otimizado

## Implementação

1. ✅ Criar estrutura de pastas (`apps/`, `packages/`, `docs/`)
2. ✅ Mover código backend existente para `apps/backend/`
3. ✅ Criar placeholder para `apps/frontend/`
4. ✅ Atualizar READMEs
5. ⏳ Ajustar CI/CD para nova estrutura
6. ⏳ Implementar aplicação frontend
7. ⏳ Criar pacotes compartilhados conforme necessidade

## Notas

- A estrutura é flexível e pode evoluir conforme necessidades
- Ferramentas como Nx, Turborepo ou Lerna podem ser adotadas no futuro se necessário
- Por enquanto, mantemos estrutura simples sem ferramentas adicionais de monorepo

## Referências

- [Monorepos: A Multivocal Literature Review](https://ieeexplore.ieee.org/document/9463082)
- [Google's Monorepo](https://research.google/pubs/pub45424/)
- [Microsoft's Monorepo Strategy](https://devblogs.microsoft.com/engineering-at-microsoft/)
