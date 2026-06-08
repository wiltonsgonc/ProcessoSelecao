USE master;
GO

DECLARE @loginName NVARCHAR(100) = '$(DB_EXTERNAL_USER)';
DECLARE @loginPassword NVARCHAR(100) = '$(DB_EXTERNAL_PASSWORD)';
DECLARE @dbName NVARCHAR(100) = '$(DB_NAME)';

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = @loginName)
BEGIN
    DECLARE @sql NVARCHAR(MAX) = 'CREATE LOGIN ' + QUOTENAME(@loginName) + ' WITH PASSWORD = ''' + @loginPassword + '''';
    EXEC sp_executesql @sql;
    PRINT 'Login created: ' + @loginName;
END
ELSE
    PRINT 'Login already exists: ' + @loginName;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '$(DB_NAME)')
BEGIN
    CREATE DATABASE [$(DB_NAME)];
    PRINT 'Database $(DB_NAME) created';
END
ELSE
    PRINT 'Database $(DB_NAME) already exists';
GO

USE [$(DB_NAME)];
GO

DECLARE @loginName NVARCHAR(100) = '$(DB_EXTERNAL_USER)';

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = @loginName)
BEGIN
    DECLARE @sql NVARCHAR(MAX) = 'CREATE USER ' + QUOTENAME(@loginName) + ' FOR LOGIN ' + QUOTENAME(@loginName);
    EXEC sp_executesql @sql;
    PRINT 'User created: ' + @loginName;
END
ELSE
    PRINT 'User already exists: ' + @loginName;
GO

DECLARE @loginName NVARCHAR(100) = '$(DB_EXTERNAL_USER)';

IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE member_principal_id = USER_ID(@loginName) AND role_principal_id = USER_ID('db_datareader'))
BEGIN
    DECLARE @sql NVARCHAR(MAX) = 'EXEC sp_addrolemember ''db_datareader'', ' + QUOTENAME(@loginName);
    EXEC sp_executesql @sql;
    PRINT 'db_datareader role granted';
END
ELSE
    PRINT 'db_datareader already granted';

IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE member_principal_id = USER_ID(@loginName) AND role_principal_id = USER_ID('db_datawriter'))
BEGIN
    DECLARE @sql2 NVARCHAR(MAX) = 'EXEC sp_addrolemember ''db_datawriter'', ' + QUOTENAME(@loginName);
    EXEC sp_executesql @sql2;
    PRINT 'db_datawriter role granted';
END
ELSE
    PRINT 'db_datawriter already granted';
GO

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
    PRINT 'Table DatabaseInfo already exists';
GO

IF NOT EXISTS (SELECT * FROM dbo.DatabaseInfo)
BEGIN
    INSERT INTO dbo.DatabaseInfo (DatabaseName) VALUES ('$(DB_NAME)');
    PRINT 'Test data inserted';
END
ELSE
    PRINT 'Test data already exists';
GO

USE master;
GO

EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'remote admin connections', 1;
RECONFIGURE;
GO
