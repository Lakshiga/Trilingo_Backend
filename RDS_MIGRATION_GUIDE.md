# RDS Database Migration Guide for Main Activities

## Problem
Main activity updates are not working in the AWS RDS database. This guide will help you apply the necessary migrations to fix the issue.

## ⚠️ Important: dotnet-ef Installation Bug
**Amazon Linux 2023 + .NET 9 has a known bug** where `dotnet-ef` cannot be installed globally. This is a Microsoft issue affecting everyone.

**✅ Solution**: Your backend already has auto-migration code built-in! Migrations will apply automatically when you start the application.

## 🚀 Quick Start (Easiest Method)

**Just restart your backend application!** Migrations apply automatically.

```bash
# On EC2, set connection string and run:
export DATABASE_CONNECTION_STRING="Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"

cd ~/Trilingo_Learning_App_Backend
dotnet run --project TES_Learning_App.API
```

That's it! The app will automatically apply any pending migrations on startup.

## Prerequisites
1. Access to AWS RDS instance: `trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com`
2. Database credentials (admin/Lachchu_16)
3. SQL Server Management Studio (SSMS) or Azure Data Studio (optional, for manual SQL)
4. Access to EC2 instance where backend is deployed

## ✅ Option 1: Automatic Migration on Startup (EASIEST - Recommended)

**Your backend already has this configured!** Migrations apply automatically when the app starts.

### Step 1: Connect to EC2 Instance
```bash
ssh -i your-key.pem ec2-user@your-ec2-ip
```

### Step 2: Navigate to Backend Directory
```bash
cd ~/Trilingo_Learning_App_Backend
# Or wherever your backend is located
```

### Step 3: Set Database Connection Environment Variable
```bash
export DATABASE_CONNECTION_STRING="Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"
```

### Step 4: Build and Run the Application
```bash
# Build the project
dotnet build TES_Learning_App.API --configuration Release

# Run the application (migrations will apply automatically)
dotnet run --project TES_Learning_App.API
```

**That's it!** The application will:
1. ✅ Check for pending migrations
2. ✅ Apply them automatically
3. ✅ Seed initial data
4. ✅ Start the API server

You'll see logs like:
```
info: Applying 1 pending database migration(s)...
info:   - 20251122073626_AddMainActivityIdToActivityType
info: ✅ Database migrations applied successfully!
```

### Step 5: Restart Backend Service (if using systemd)
```bash
# If running as a service
sudo systemctl restart trilingo-backend

# Check status
sudo systemctl status trilingo-backend

# View logs
sudo journalctl -u trilingo-backend -f
```

---

## Option 2: Manual Migration via EF Tools (Workaround for dotnet-ef Bug)

**⚠️ Note**: You cannot install `dotnet-ef` globally on Amazon Linux 2023 + .NET 9 due to a Microsoft bug. Use these workarounds instead.

### Method A: Use EF Tools from Project (Recommended Workaround)

Your project already includes `Microsoft.EntityFrameworkCore.Tools` (version 9.0.9), so you can use it directly without global installation.

#### Step 1: Connect to EC2 Instance
```bash
ssh -i your-key.pem ec2-user@your-ec2-ip
```

#### Step 2: Navigate to Backend Directory
```bash
cd ~/Trilingo_Learning_App_Backend
# Or wherever your backend is located
```

#### Step 3: Set Environment Variable
```bash
export DATABASE_CONNECTION_STRING="Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"
```

#### Step 4: Build the Project First
```bash
dotnet build TES_Learning_App.API --configuration Release
```

#### Step 5: Run Migration Using EF DLL Directly

Find the EF tools DLL in the build output:
```bash
# Find the EF tools DLL (usually in bin/Debug or bin/Release)
find ~/Trilingo_Learning_App_Backend -name "ef.dll" -type f

# Or check the Infrastructure project bin folder
ls -la ~/Trilingo_Learning_App_Backend/TES_Learning_App.Infrastructure/bin/Release/net9.0/
```

Then run migration using the EF DLL:
```bash
# Replace net9.0 with your actual framework version if different
dotnet exec ~/Trilingo_Learning_App_Backend/TES_Learning_App.Infrastructure/bin/Release/net9.0/tools/net9.0/any/ef.dll database update \
  --project TES_Learning_App.Infrastructure \
  --startup-project TES_Learning_App.API \
  --configuration Release
```

**Alternative**: Use MSBuild task (if available):
```bash
dotnet msbuild TES_Learning_App.API /t:EntityFrameworkMigrations
```

### Method B: Use dotnet ef via Local Tools (Alternative)

If the above doesn't work, try installing EF tools as a local tool:

```bash
# Navigate to backend root
cd ~/Trilingo_Learning_App_Backend

# Create tool manifest (if doesn't exist)
dotnet new tool-manifest

# Install EF tools as local tool
dotnet tool install --local dotnet-ef

# Run migration
dotnet ef database update --project "TES_Learning_App.Infrastructure" --startup-project "TES_Learning_App.API"
```

**Note**: This may still fail due to the .NET 9 bug. If it does, use Method A or Option 1 (automatic migration).

### Step 5: Restart Backend Service
```bash
# If using systemd
sudo systemctl restart trilingo-backend

# Or if running manually
# Stop the current process and restart
```

## Option 3: Apply Migrations via SQL Script (Direct to RDS)

### Step 1: Connect to RDS Database
Use SQL Server Management Studio or Azure Data Studio:
- **Server**: `trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433`
- **Authentication**: SQL Server Authentication
- **Login**: `admin`
- **Password**: `Lachchu_16`
- **Database**: `Trilingo_Learning_Db`

### Step 2: Verify MainActivities Table Exists
```sql
-- Check if MainActivities table exists
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'MainActivities';

-- Check table structure
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'MainActivities'
ORDER BY ORDINAL_POSITION;
```

### Step 3: Verify Table Has Correct Structure
The MainActivities table should have:
- `Id` (int, Primary Key, Identity)
- `Name_en` (nvarchar(100), NOT NULL)
- `Name_ta` (nvarchar(100), NOT NULL)
- `Name_si` (nvarchar(100), NOT NULL)

If the table doesn't exist or is missing columns, run:

```sql
-- Create MainActivities table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MainActivities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MainActivities] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name_en] NVARCHAR(100) NOT NULL,
        [Name_ta] NVARCHAR(100) NOT NULL,
        [Name_si] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_MainActivities] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'MainActivities table created successfully';
END
ELSE
BEGIN
    PRINT 'MainActivities table already exists';
END

-- Verify the table structure
SELECT 
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.IS_NULLABLE,
    c.CHARACTER_MAXIMUM_LENGTH,
    CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE 'NO' END AS IS_PRIMARY_KEY
FROM INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN (
    SELECT ku.TABLE_CATALOG, ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku
        ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' 
        AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
) pk ON c.TABLE_CATALOG = pk.TABLE_CATALOG
    AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA
    AND c.TABLE_NAME = pk.TABLE_NAME
    AND c.COLUMN_NAME = pk.COLUMN_NAME
WHERE c.TABLE_NAME = 'MainActivities'
ORDER BY c.ORDINAL_POSITION;
```

### Step 4: Check for Pending Migrations
```sql
-- Check if __EFMigrationsHistory table exists
SELECT * FROM [__EFMigrationsHistory] 
ORDER BY MigrationId;

-- Expected migrations:
-- 20251107100231_InitialCreate
-- 20251107102052_AddJsonMethodToActivityType
-- 20251107131515_AddExercises
-- 20251109155926_AddProfileImageUrlToUser
-- 20251122073626_AddMainActivityIdToActivityType
```

### Step 5: Apply Missing Migrations
If migrations are missing, you can manually apply them. However, it's recommended to use the EF Core migration tool.

## Option 4: Verify Automatic Migration is Working

The application is configured to automatically apply pending migrations on startup (see `Program.cs` and `DbInitializer.cs`). This requires:

1. **Backend must have proper connection string** configured in `appsettings.Aws.json` or environment variable
2. **Backend must have database access** from EC2 security group
3. **Backend must be restarted** after code deployment

### Verify Auto-Migration is Working

Check the backend logs on EC2:
```bash
# View application logs
sudo journalctl -u trilingo-backend -f

# Or if running manually, check console output for:
# "Applying migration..." or "An error occurred while seeding the database."
```

## Verification Steps

After applying migrations, verify everything is working:

### 1. Test Database Connection
```sql
-- Test query
SELECT TOP 5 * FROM MainActivities;
```

### 2. Test Update Functionality
1. Go to AWS hosted admin panel: https://d3v81eez8ecmto.cloudfront.net/main-activities
2. Click Edit on any main activity
3. Modify the name fields
4. Click Save
5. Verify the changes are persisted

### 3. Check Backend Logs
Monitor the backend logs for any errors:
```bash
# On EC2
sudo journalctl -u trilingo-backend -f --since "5 minutes ago"
```

## Troubleshooting

### Issue: "dotnet-ef: command not found" or "Could not execute because the application was not found"
**Cause**: Amazon Linux 2023 + .NET 9 bug prevents global dotnet-ef installation.

**Solution**: 
- ✅ **Use Option 1 (Automatic Migration)** - Just restart your backend app
- ✅ **Use Option 2 Method A** - Use EF tools from project without global installation
- ❌ **Don't try to install dotnet-ef globally** - It won't work due to Microsoft bug

### Issue: "MainActivity not found" error
**Solution**: Verify the MainActivities table exists and has data:
```sql
SELECT * FROM MainActivities;
```

### Issue: "Cannot insert NULL" error
**Solution**: Ensure all required fields are provided in the update request. Check the frontend is sending all three name fields.

### Issue: Updates not persisting
**Possible causes**:
1. Database connection issue - Check connection string
2. Transaction not committing - Check `CompleteAsync()` is being called
3. Caching issue - Clear browser cache and try again
4. Wrong database - Verify you're connected to the correct RDS instance
5. Migrations not applied - Check if migrations were applied successfully

### Issue: Migration fails with "table already exists"
**Solution**: This is normal if the table already exists. The migration will skip creating it.

### Issue: Connection timeout
**Solution**: 
1. Check RDS security group allows connections from EC2
2. Verify connection string is correct
3. Check RDS instance is running and accessible
4. Verify network connectivity: `telnet trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com 1433`

### Issue: "No migrations found" but table doesn't exist
**Solution**: 
1. Check if migrations folder exists: `ls TES_Learning_App.Infrastructure/Migrations/`
2. Verify migration files are included in the build
3. Try Option 3 (SQL Script) to manually create the table

### Issue: Automatic migration not running on startup
**Solution**:
1. Check application logs for migration messages
2. Verify `Program.cs` has the migration code (it should be there)
3. Check if `DbInitializer.Initialize()` is being called
4. Verify database connection string is set correctly
5. Check for exceptions in logs that might prevent migration from running

## Quick Fix Script

If you just need to ensure the MainActivities table is properly set up, run this SQL:

```sql
USE [Trilingo_Learning_Db];
GO

-- Ensure MainActivities table exists with correct structure
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MainActivities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[MainActivities] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name_en] NVARCHAR(100) NOT NULL,
        [Name_ta] NVARCHAR(100) NOT NULL,
        [Name_si] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_MainActivities] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'MainActivities table created';
END
ELSE
BEGIN
    -- Add missing columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MainActivities]') AND name = 'Name_en')
    BEGIN
        ALTER TABLE [dbo].[MainActivities] ADD [Name_en] NVARCHAR(100) NOT NULL DEFAULT '';
        PRINT 'Added Name_en column';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MainActivities]') AND name = 'Name_ta')
    BEGIN
        ALTER TABLE [dbo].[MainActivities] ADD [Name_ta] NVARCHAR(100) NOT NULL DEFAULT '';
        PRINT 'Added Name_ta column';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MainActivities]') AND name = 'Name_si')
    BEGIN
        ALTER TABLE [dbo].[MainActivities] ADD [Name_si] NVARCHAR(100) NOT NULL DEFAULT '';
        PRINT 'Added Name_si column';
    END
    
    PRINT 'MainActivities table structure verified';
END
GO

-- Verify table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'MainActivities'
ORDER BY ORDINAL_POSITION;
GO
```

## Next Steps

After applying migrations:
1. ✅ Restart the backend service on EC2
2. ✅ Test the update functionality in the admin panel
3. ✅ Monitor logs for any errors
4. ✅ Verify data is persisting correctly

## Support

If issues persist:
1. Check backend application logs
2. Verify RDS security group configuration
3. Test database connection directly
4. Review the code changes in this commit for MainActivity update functionality

