# Agente de Modificações

## Modificações Realizadas

### 1. Correção de bugs no cálculo de prazo (ProcessoSelecao.cs e ProcessoSelecaoService.cs)
- **Problema**: O cálculo de prazo considerava apenas a data, ignorando a hora. Isso causava a marcação incorreta de processos como "Finalizado" quando o prazo ainda era válido.
- **Solução**:
  - Alterado `VerificarPrazoExpirado` para usar `DateTime.Now > DataFim.Value` em vez de comparar apenas com o final do dia.
  - Renomeado `EstaWithinPrazo` para `EstaDentroDoPrazo` e removido a verificação de status, considerando apenas as datas.
  - Adicionado método `ReverterSePrazoValido` que reverte o status de `Finalizado` para `EmAndamento` se o prazo ainda for válido.
  - Atualizado `GetAllAsync` e `GetByIdAsync` para sempre verificar e atualizar o status conforme o prazo atual.

### 2. Reset de IDENTITY das tabelas
- **Problema**: Os IDs estavam começando em valores altos (possivelmente de registros anteriores).
- **Solução**:
  - Conectado ao SQL Server via programa .NET executado no container `processo-selecao-backend`.
  - Executados comandos `DELETE` e `DBCC CHECKIDENT` para redefinir o `IDENTITY` das tabelas:
    - `ProcessosSelecao`
    - `Candidatos`
    - `Avaliadores`
    - `Documentos`
    - `Baremas`
  - Verificado que todas as tabelas ficaram com 0 registros e o `IDENTITY` foi redefinido para começar em 0 (próximo ID será 1).

## Arquivos Criados/Modificados
- `src/backend/ProcessoSelecao.Domain/Entities/ProcessoSelecao.cs`: Correção de cálculo de prazo e adição de `ReverterSePrazoValido`.
- `src/backend/ProcessoSelecao.Application/Services/ProcessoSelecaoService.cs`: Atualização dos métodos `GetAllAsync`, `GetByIdAsync` e `IniciarAsync`.
- `reset-db-docker.bat`: Script para resetar o banco usando Docker (caso necessário no futuro).

## Próximos Passos
- Os IDs sequenciais começarão a partir de 1 ao criar novos processos.
- A lógica de prazo agora considera a data e hora exatas, evitando marcações incorretas de "Finalizado".