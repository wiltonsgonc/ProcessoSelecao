@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   Reset Banco de Dados - ProcessoSelecao
echo ========================================
echo.

:: Carregar variáveis do .env
for /f "usebackq tokens=*" %%a in ("%CD%\.env") do set "%%a"

echo [1/2] Resetando IDENTITY da tabela ProcessosSelecao...
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd ^
  -S localhost -U sa -P "Processo@123" -C ^
  -Q "USE ProcessoSelecao; DELETE FROM ProcessosSelecao; DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);"

echo.
echo [2/2] Resetando IDENTITY das outras tabelas...
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd ^
  -S localhost -U sa -P "Processo@123" -C ^
  -Q "USE ProcessoSelecao; DELETE FROM Candidatos; DBCC CHECKIDENT ('Candidatos', RESEED, 0);"

podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd ^
  -S localhost -U sa -P "Processo@123" -C ^
  -Q "USE ProcessoSelecao; DELETE FROM Avaliadores; DBCC CHECKIDENT ('Avaliadores', RESEED, 0);"

podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd ^
  -S localhost -U sa -P "Processo@123" -C ^
  -Q "USE ProcessoSelecao; DELETE FROM Documentos; DBCC CHECKIDENT ('Documentos', RESEED, 0);"

podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd ^
  -S localhost -U sa -P "Processo@123" -C ^
  -Q "USE ProcessoSelecao; DELETE FROM Baremas; DBCC CHECKIDENT ('Baremas', RESEED, 0);"

echo.
echo ========================================
echo   Reset concluido com sucesso!
echo ========================================
echo.
