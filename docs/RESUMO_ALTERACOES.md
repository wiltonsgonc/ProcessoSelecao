# Resumo das Alterações

## Problemas Identificados

1. **ID com números altos**: O sistema estava gerando IDs altos (ex: 20001) em vez de começar em 1
2. **Inconsistência de timezone**: Uso de `DateTime.Now` em vez de `DateTime.UtcNow` causava problemas
3. **Prazo não terminava às 23:59:59**: O prazo precisava terminar no final do dia

## Alterações Realizadas

### 1. Reset do Identity Seed no Banco de Dados
- Executado programa `ResetIdentity` para deletar todos os registros
- Resetado o identity seed de todas as tabelas para 0 (próximo ID será 1)
- Tabelas afetadas: ProcessosSelecao, Candidatos, Avaliadores, Baremas, Documentos

### 2. Correção do DateTime.Now para DateTime.UtcNow
Arquivos alterados:
- `src/backend/ProcessoSelecao.Domain/Entities/ProcessoSelecao.cs`
  - `DataInicio = DateTime.Now` → `DataInicio = DateTime.UtcNow`
  - `DataFim = DateTime.Now` → `DataFim = DateTime.UtcNow`
  - `DateTime.Now > DataFim.Value` → `DateTime.UtcNow > DataFim.Value`
  - `var now = DateTime.Now` → `var now = DateTime.UtcNow`

- `src/backend/ProcessoSelecao.Application/Services/ProcessoSelecaoService.cs`
  - `DateTime.Now > entity.DataFim.Value` → `DateTime.UtcNow > entity.DataFim.Value`

### 3. Correção do Prazo às 23:59:59 (Mantida)
- `src/backend/ProcessoSelecao.Application/MappingProfile.cs`
  - Mapeamento de `DataFim` para definir como 23:59:59 do dia selecionado

## Como Verificar a Correção

1. **Reinicie a aplicação backend** para pegar as novas configurações
2. **Crie um novo processo** de seleção
3. **Verifique se o ID começa em 1** (e não em 20001)
4. **Verifique se o prazo termina às 23:59:59** do dia selecionado

## Script SQL para Resetar IDs (se necessário)

O programa `ResetIdentity` já foi executado, mas se precisar resetar novamente:

```sql
USE ProcessoSelecaoDb;
GO

DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);
DBCC CHECKIDENT ('Candidatos', RESEED, 0);
DBCC CHECKIDENT ('Avaliadores', RESEED, 0);
DBCC CHECKIDENT ('Baremas', RESEED, 0);
DBCC CHECKIDENT ('Documentos', RESEED, 0);
```

## Notas Importantes

- O banco de dados está atualmente vazio (todos os registros foram deletados)
- O identity seed foi resetado para 0 em todas as tabelas
- O próximo ID gerado será 1
- A correção do prazo às 23:59:59 está mantida
