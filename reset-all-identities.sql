-- Script para resetar todas as sequências de IDs no banco de dados
USE ProcessoSelecaoDb;
GO

-- Resetar identity de todas as tabelas
DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);
DBCC CHECKIDENT ('Candidatos', RESEED, 0);
DBCC CHECKIDENT ('Avaliadores', RESEED, 0);
DBCC CHECKIDENT ('Baremas', RESEED, 0);
DBCC CHECKIDENT ('Documentos', RESEED, 0);

-- Verificar os novos seeds
DBCC CHECKIDENT ('ProcessosSelecao', NORESEED);
DBCC CHECKIDENT ('Candidatos', NORESEED);
DBCC CHECKIDENT ('Avaliadores', NORESEED);
DBCC CHECKIDENT ('Baremas', NORESEED);
DBCC CHECKIDENT ('Documentos', NORESEED);
GO
