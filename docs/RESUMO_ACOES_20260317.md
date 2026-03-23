# Resumo das Ações Realizadas em 2026-03-17

## 1. Correção de Bug no Cálculo de Prazo
- **Arquivo**: `src/backend/ProcessoSelecao.Domain/Entities/ProcessoSelecao.cs`
- **Alterações**:
  - `VerificarPrazoExpirado`: Usa `DateTime.Now > DataFim.Value` em vez de comparar apenas com o final do dia.
  - `EstaDentroDoPrazo`: Renomeado de `EstaWithinPrazo` e considera apenas datas, não status.
  - `ReverterSePrazoValido`: Novo método que reverte status de `Finalizado` para `EmAndamento` se o prazo ainda for válido.
- **Arquivo**: `src/backend/ProcessoSelecao.Application/Services/ProcessoSelecaoService.cs`
- **Alterações**:
  - `GetAllAsync` e `GetByIdAsync`: Atualizam status automaticamente conforme o prazo.
  - `IniciarAsync`: Corrigida verificação de prazo expirado.

## 2. Reset de IDENTITY das Tabelas
- **Ação**: Executado comando `DBCC CHECKIDENT` para redefinir o `IDENTITY` de todas as tabelas.
- **Resultado**: IDs sequenciais começam em 1.

## 3. Correção de Erro ao Criar Processo
- **Problema**: Backend travado devido ao `dotnet watch`.
- **Solução**: Modificado `docker-compose.yml` para usar `Dockerfile.prod` (sem `dotnet watch`).
- **Arquivo**: `docker-compose.yml` linha 43: `dockerfile: ProcessoSelecao.Api/Dockerfile.prod`
- **Novo Dockerfile**: `src/backend/ProcessoSelecao.Api/Dockerfile.prod`

## 4. Remoção de Debugs
- **Backend**: Removido arquivo temporário `ResetIdentity.cs`.
- **Frontend**: Comentados `console.log` em:
  - `src/frontend/src/app/modules/processos/processo-list/processo-list.component.ts`
  - `src/frontend/src/app/modules/formulario/formulario-inscricao/formulario-inscricao.component.ts`
- **Nota**: `console.error` mantidos para depuração de erros.

## 5. Organização de Documentação
- **Pasta**: `docs/`
- **Arquivos movidos**:
  - `AGENTS.md` (para `docs/AGENTS.md`)
  - `DATABASE_ACCESS.md` (já existia)
  - `IMPLEMENTATION_SUMMARY.md` (já existia)
  - `BUGFIXES_20260317.md` (novo)
  - `RESUMO_ACOES_20260317.md` (novo)
- **Arquivo removido**: `RESET_IDENTITY.md` (não era necessário).

## Status Final
- ✅ Backend rodando na porta 5002.
- ✅ Frontend rodando na porta 4200.
- ✅ Criação de processos funciona com IDs sequenciais a partir de 1.
- ✅ Lógica de prazo corrigida (considera data e hora).
- ✅ Debugs removidos.
- ✅ Documentação organizada.