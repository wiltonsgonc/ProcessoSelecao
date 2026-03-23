-- Resetar a sequência de IDs da tabela ProcessosSelecao
USE ProcessoSelecaoDb;
GO

-- Verificar o maior ID atual
SELECT MAX(Id) as MaxId FROM ProcessosSelecao;
GO

-- Resetar a sequência de IDs para começar do último ID + 1
-- Primeiro, deletar todos os registros (se necessário)
-- DELETE FROM ProcessosSelecao;
-- GO

-- Resetar o identity seed
DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);
GO

-- Verificar o novo seed
DBCC CHECKIDENT ('ProcessosSelecao', NORESEED);
GO
