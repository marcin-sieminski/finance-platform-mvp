#!/bin/bash
# Custom entrypoint for SQL Server container.
# Starts sqlservr in background, waits for it to accept connections,
# runs init-db.sql as 'sa', then hands off to the SQL Server process.

set -e

/opt/mssql/bin/sqlservr &
PID=$!

echo "[entrypoint] Waiting for SQL Server to start..."
READY=0
for i in $(seq 1 30); do
    /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
        -Q "SELECT 1" -C -b > /dev/null 2>&1 && { READY=1; break; }
    echo "[entrypoint] Attempt $i: not ready yet, retrying in 2s..."
    sleep 2
done

if [ "$READY" -eq 0 ]; then
    echo "[entrypoint] FATAL: SQL Server failed to start after 60 seconds."
    exit 1
fi

echo "[entrypoint] Creating login..."
/opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -b \
    -Q "IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'financeapp') CREATE LOGIN [financeapp] WITH PASSWORD = N'${FINANCEAPP_PASSWORD}';"

echo "[entrypoint] Running init-db.sql..."
/opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
    -i /init-db.sql -C -b

echo "[entrypoint] Initialization complete."
wait $PID
