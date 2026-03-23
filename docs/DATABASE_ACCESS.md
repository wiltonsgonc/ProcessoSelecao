# Database Access Configuration

This document describes how to access the SQL Server database in the ProcessoSelecao application.

## Database Connection Details

### SQL Server Container
- **Host**: `localhost` (when running locally)
- **Port**: `1433`
- **Database Name**: `ProcessoSelecaoDb`

### Connection Strings

#### For DBeaver and other SQL clients:
```
Server: localhost,1433
Database: ProcessoSelecaoDb
Authentication: SQL Server Authentication
Username: db_user
Password: DbUser@123
```

#### For SQL Server Management Studio (SSMS):
```
Server name: localhost,1433
Authentication: SQL Server Authentication
Login: db_user
Password: DbUser@123
```

#### For Command Line (sqlcmd):

You can use any SQL Server client tool. The database is accessible at:
- **Host**: `localhost`
- **Port**: `1433`
- **Database**: `ProcessoSelecaoDb`
- **Username**: `db_user`
- **Password**: `DbUser@123`

If you have SQL Server command line tools installed on your host machine:

```bash
# Connect using sqlcmd
sqlcmd -S localhost,1433 -U db_user -P "DbUser@123" -d ProcessoSelecaoDb

# Run queries
SELECT TOP 10 * FROM Candidatos;
GO
```

## Database Users

### Administrative User (SA)
- **Username**: `sa`
- **Password**: `Processo@123`
- **Purpose**: Full administrative access, use only for maintenance

### Application User
- **Username**: `db_user`
- **Password**: `DbUser@123`
- **Purpose**: Read/write access for application operations
- **Permissions**: `db_datareader`, `db_datawriter`

## Database Persistence

The database data is persisted using Docker volumes:
- `sqlserver_data`: Database files
- `sqlserver_log`: Transaction logs
- `sqlserver_backup`: Backup files

These volumes ensure that your database persists even when containers are restarted or recreated.

## Accessing the Database Console

### Method 1: Using sqlcmd inside the SQL Server container

```bash
# Access the SQL Server container
docker exec -it processo-selecao-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Processo@123"

# Run SQL commands
SELECT name FROM sys.databases;
USE ProcessoSelecaoDb;
SELECT * FROM Candidatos;
```

### Method 2: Using DBeaver (Recommended)

1. Install DBeaver (https://dbeaver.io/)
2. Create a new SQL Server connection
3. Use the connection details above
4. Connect and browse the database

### Method 3: Using Azure Data Studio

1. Install Azure Data Studio (https://aka.ms/azuredatastudio)
2. Create a new connection
3. Use the connection details above
4. Connect and browse the database

## Database Backup and Restore

### Creating a Backup

```bash
# From your host machine
docker exec -it processo-selecao-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "Processo@123" \
  -Q "BACKUP DATABASE [ProcessoSelecaoDb] TO DISK = '/var/opt/mssql/backup/ProcessoSelecaoDb.bak' WITH COMPRESSION, STATS = 5"
```

### Restoring a Backup

```bash
# From your host machine
docker exec -it processo-selecao-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "Processo@123" \
  -Q "RESTORE DATABASE [ProcessoSelecaoDb] FROM DISK = '/var/opt/mssql/backup/ProcessoSelecaoDb.bak' WITH REPLACE, STATS = 5"
```

## Database Schema

The database schema is defined using Entity Framework Core migrations. To view or create migrations:

```bash
# Navigate to the backend directory
cd src/backend/ProcessoSelecao.Api

# View existing migrations
dotnet ef migrations list

# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update
```

## Troubleshooting

### Connection Issues

1. **Verify SQL Server is running**:
   ```bash
   docker ps | grep sqlserver
   ```

2. **Check container logs**:
   ```bash
   docker logs processo-selecao-sqlserver
   ```

3. **Test connection from host**:
   ```bash
   telnet localhost 1433
   ```

4. **Reset SA password** (if needed):
   ```bash
   docker exec -it processo-selecao-sqlserver /opt/mssql-tools/bin/sqlcmd \
     -S localhost -U sa -P "oldpassword" \
     -Q "ALTER LOGIN sa WITH PASSWORD = 'Processo@123'"
   ```

### Permission Issues

If you encounter permission errors with the `db_user`:

```bash
# Grant additional permissions
docker exec -it processo-selecao-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P "Processo@123" \
  -d ProcessoSelecaoDb \
  -Q "EXEC sp_addrolemember 'db_owner', 'db_user'"
```

## Initialization Script

The `init.sql` file is executed automatically when the SQL Server container starts for the first time. It:

1. Creates the `db_user` with appropriate permissions
2. Creates a test table `DatabaseInfo` for verification
3. Enables remote admin connections

You can modify this file to add custom initialization logic as needed.
