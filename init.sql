-- Criar um usuário dedicado para acesso externo (ex: DBeaver)
USE master;
GO

-- Criar um usuário de banco de dados com senha
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'db_user')
BEGIN
    CREATE LOGIN db_user WITH PASSWORD = 'DbUser@123';
    PRINT 'Database user db_user created';
END
ELSE
BEGIN
    PRINT 'Database user db_user already exists';
END
GO

-- Criar o banco de dados ProcessoSelecaoDb se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ProcessoSelecaoDb')
BEGIN
    CREATE DATABASE ProcessoSelecaoDb;
    PRINT 'Database ProcessoSelecaoDb created';
END
ELSE
BEGIN
    PRINT 'Database ProcessoSelecaoDb already exists';
END
GO

-- Conceder acesso ao banco de dados ProcessoSelecaoDb
USE ProcessoSelecaoDb;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_user')
BEGIN
    CREATE USER db_user FOR LOGIN db_user;
    PRINT 'Database user db_user added to ProcessoSelecaoDb';
END
ELSE
BEGIN
    PRINT 'Database user db_user already exists in ProcessoSelecaoDb';
END
GO

-- Conceder permissões necessárias
IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE member_principal_id = USER_ID('db_user') AND role_principal_id = USER_ID('db_datareader'))
BEGIN
    EXEC sp_addrolemember 'db_datareader', 'db_user';
    PRINT 'db_datareader role granted to db_user';
END
ELSE
BEGIN
    PRINT 'db_datareader role already granted to db_user';
END

IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE member_principal_id = USER_ID('db_user') AND role_principal_id = USER_ID('db_datawriter'))
BEGIN
    EXEC sp_addrolemember 'db_datawriter', 'db_user';
    PRINT 'db_datawriter role granted to db_user';
END
ELSE
BEGIN
    PRINT 'db_datawriter role already granted to db_user';
END
GO

-- Criar uma tabela de teste simples para verificação
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.DatabaseInfo') AND type in ('U'))
BEGIN
    CREATE TABLE dbo.DatabaseInfo (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        DatabaseName NVARCHAR(100),
        CreatedDate DATETIME DEFAULT GETDATE()
    );
    PRINT 'Table DatabaseInfo created';
END
ELSE
BEGIN
    PRINT 'Table DatabaseInfo already exists';
END
GO

-- Inserir dados de teste
IF NOT EXISTS (SELECT * FROM dbo.DatabaseInfo)
BEGIN
    INSERT INTO dbo.DatabaseInfo (DatabaseName) VALUES ('ProcessoSelecaoDb');
    PRINT 'Test data inserted';
END
ELSE
BEGIN
    PRINT 'Test data already exists';
END
GO

-- Habilitar modo de autenticação do SQL Server
USE master;
GO

EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'remote admin connections', 1;
RECONFIGURE;
GO