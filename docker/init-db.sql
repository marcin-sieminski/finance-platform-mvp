-- Idempotent init script: creates database, database user, and grants permissions.
-- Login creation is handled in entrypoint.sh (password via bash variable expansion).
-- Safe to re-run on container restart (all statements guarded with IF NOT EXISTS).

-- 1. Create database (EF migrations will create/update the schema)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FinancePlatform')
    CREATE DATABASE FinancePlatform;
GO

USE FinancePlatform;
GO

-- 2. Create database user mapped to login
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'financeapp')
    CREATE USER financeapp FOR LOGIN financeapp;
GO

-- 3. Grant role memberships (guarded — ALTER ROLE throws 15405 if member already exists)
IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datareader' AND m.name = 'financeapp'
)
    ALTER ROLE db_datareader ADD MEMBER financeapp;

IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datawriter' AND m.name = 'financeapp'
)
    ALTER ROLE db_datawriter ADD MEMBER financeapp;

IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = 'db_ddladmin' AND m.name = 'financeapp'
)
    ALTER ROLE db_ddladmin ADD MEMBER financeapp;
GO

-- 4. Object-level grants (GRANT is idempotent — safe to re-run)
GRANT CREATE TABLE     TO financeapp;
GRANT ALTER ANY SCHEMA TO financeapp;
GO
