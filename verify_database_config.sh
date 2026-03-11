#!/bin/bash

echo "=========================================="
echo "Database Configuration Verification"
echo "=========================================="
echo ""

# Check if docker-compose.yml exists
if [ -f "docker-compose.yml" ]; then
    echo "✓ docker-compose.yml found"
else
    echo "✗ docker-compose.yml not found"
    exit 1
fi

# Check if init.sql exists
if [ -f "init.sql" ]; then
    echo "✓ init.sql found"
else
    echo "✗ init.sql not found"
    exit 1
fi

# Check if entrypoint.sh exists
if [ -f "entrypoint.sh" ]; then
    echo "✓ entrypoint.sh found"
else
    echo "✗ entrypoint.sh not found"
    exit 1
fi

# Check if .env exists
if [ -f ".env" ]; then
    echo "✓ .env found"
else
    echo "✗ .env not found"
    exit 1
fi

# Check if Dockerfile exists
if [ -f "src/backend/ProcessoSelecao.Api/Dockerfile" ]; then
    echo "✓ Dockerfile found"
else
    echo "✗ Dockerfile not found"
    exit 1
fi

echo ""
echo "=========================================="
echo "Verifying docker-compose configuration"
echo "=========================================="
echo ""

# Validate docker-compose configuration
if docker-compose config > /dev/null 2>&1; then
    echo "✓ docker-compose.yml is valid"
else
    echo "✗ docker-compose.yml has errors"
    docker-compose config
    exit 1
fi

echo ""
echo "=========================================="
echo "Checking database volumes"
echo "=========================================="
echo ""

# Check if volumes are defined
if grep -q "sqlserver_data:" docker-compose.yml && \
   grep -q "sqlserver_log:" docker-compose.yml && \
   grep -q "sqlserver_backup:" docker-compose.yml; then
    echo "✓ All database volumes are defined"
else
    echo "✗ Some database volumes are missing"
    exit 1
fi

echo ""
echo "=========================================="
echo "Checking database user configuration"
echo "=========================================="
echo ""

# Check if external user is configured
if grep -q "DB_EXTERNAL_USER" .env && grep -q "DB_EXTERNAL_PASSWORD" .env; then
    echo "✓ External database user is configured"
else
    echo "✗ External database user is not configured"
    exit 1
fi

echo ""
echo "=========================================="
echo "Checking init.sql content"
echo "=========================================="
echo ""

# Check if init.sql has required content
if grep -q "CREATE LOGIN db_user" init.sql && \
   grep -q "CREATE USER db_user" init.sql && \
   grep -q "sp_addrolemember" init.sql; then
    echo "✓ init.sql has required database user setup"
else
    echo "✗ init.sql is missing required content"
    exit 1
fi

echo ""
echo "=========================================="
echo "Checking entrypoint.sh content"
echo "=========================================="
echo ""

# Check if entrypoint.sh has required content
if grep -q "nc -z" entrypoint.sh && \
   grep -q "dotnet ef database update" entrypoint.sh; then
    echo "✓ entrypoint.sh has required database initialization"
else
    echo "✗ entrypoint.sh is missing required content"
    exit 1
fi

echo ""
echo "=========================================="
echo "Checking Dockerfile content"
echo "=========================================="
echo ""

# Check if Dockerfile has required content
if grep -q "dotnet-ef" src/backend/ProcessoSelecao.Api/Dockerfile && \
   grep -q "entrypoint.sh" src/backend/ProcessoSelecao.Api/Dockerfile; then
    echo "✓ Dockerfile has required database tools and entrypoint"
else
    echo "✗ Dockerfile is missing required content"
    exit 1
fi

echo ""
echo "=========================================="
echo "All checks passed! ✓"
echo "=========================================="
echo ""
echo "Summary:"
echo "- Database persistence: Configured with 3 volumes"
echo "- External database user: Created (db_user/DbUser@123)"
echo "- Initialization script: Ready (init.sql)"
echo "- Entry point script: Ready (entrypoint.sh)"
echo "- Docker configuration: Valid"
echo ""
echo "You can now:"
echo "1. Start the services: docker-compose up -d"
echo "2. Connect with DBeaver using db_user/DbUser@123"
echo "3. Access via command line: sqlcmd -S localhost,1433 -U db_user -P \"DbUser@123\" -d ProcessoSelecaoDb"
echo ""
