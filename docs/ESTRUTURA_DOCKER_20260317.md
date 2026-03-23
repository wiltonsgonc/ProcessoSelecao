# Estrutura Docker - 2026-03-17

## Dockerfiles Separados

### Dockerfile.dev (Desenvolvimento)
- **Localização**: `src/backend/ProcessoSelecao.Api/Dockerfile.dev`
- **Comando**: `dotnet run` (sem `dotnet watch` para evitar erros)
- **Porta**: 5002
- **Uso**: `docker-compose up backend` (ambiente de desenvolvimento)

### Dockerfile.prod (Produção)
- **Localização**: `src/backend/ProcessoSelecao.Api/Dockerfile.prod`
- **Comando**: `dotnet ProcessoSelecao.Api.dll`
- **Porta**: 5002
- **Uso**: Imagem otimizada para produção

## docker-compose.yml
- **Backend**: Usa `Dockerfile.dev` por padrão.
- **Frontend**: Usa `Dockerfile` (Angular).
- **SQL Server**: Imagem oficial do SQL Server 2022.

## Alterações Realizadas

### 1. Correção de Bug no Cálculo de Prazo
- Arquivos: `ProcessoSelecao.cs` e `ProcessoSelecaoService.cs`
- Corrigido cálculo de prazo para considerar data e hora exatas.

### 2. Reset de IDENTITY
- Executado `DBCC CHECKIDENT` para todas as tabelas.
- IDs sequenciais começam em 1.

### 3. Remoção de Debugs
- Backend: Removido arquivo temporário `ResetIdentity.cs`.
- Frontend: Comentados `console.log` em dois componentes.

### 4. Organização de Documentação
- Pasta `docs/` contém todos os arquivos `.md` (exceto `README.md`).

## Status Atual
- ✅ Backend: `http://localhost:5002` operacional com `dotnet run`.
- ✅ IDs sequenciais: 1, 2, 3, 4...
- ✅ Dockerfiles separados para dev e prod.
- ✅ Debugs removidos.

## Notas
- O `dotnet watch` foi removido do `Dockerfile.dev` devido a erros de "item with the same key".
- Para hot reload, executar `dotnet watch` localmente (fora do Docker).