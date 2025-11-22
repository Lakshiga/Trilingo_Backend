# Database Migration Instructions

## Problem
The database doesn't have the `MainActivityId` column in the `ActivityTypes` table. This migration will add it.

## Steps to Apply Migration

### Option 1: Using Package Manager Console (Recommended)

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Set the **Default project** to `TES_Learning_App.Infrastructure`
4. Run the following command:

```powershell
Update-Database
```

### Option 2: Using .NET CLI

1. Open Command Prompt or PowerShell
2. Navigate to the backend folder:
```cmd
cd "C:\Users\TFF_DEAD\Desktop\New folder\Trilingo_Backend"
```

3. Run the migration:
```cmd
dotnet ef database update --project "TES_Learning_App.Infrastructure" --startup-project "TES_Learning_App.API"
```

### Option 3: Manual SQL (If migration fails)

If the migration fails due to existing data, you can run this SQL manually in SQL Server Management Studio:

```sql
-- Step 1: Add the column as nullable
ALTER TABLE ActivityTypes
ADD MainActivityId INT NULL;

-- Step 2: Set default value for existing records
UPDATE ActivityTypes 
SET MainActivityId = (SELECT TOP 1 Id FROM MainActivities ORDER BY Id)
WHERE MainActivityId IS NULL;

-- Step 3: Make it NOT NULL
ALTER TABLE ActivityTypes
ALTER COLUMN MainActivityId INT NOT NULL;

-- Step 4: Create index
CREATE INDEX IX_ActivityTypes_MainActivityId ON ActivityTypes(MainActivityId);

-- Step 5: Add foreign key
ALTER TABLE ActivityTypes
ADD CONSTRAINT FK_ActivityTypes_MainActivities_MainActivityId
FOREIGN KEY (MainActivityId) REFERENCES MainActivities(Id);
```

## Verify Migration

After running the migration, verify it worked:

```sql
-- Check if column exists
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ActivityTypes' AND COLUMN_NAME = 'MainActivityId';

-- Check foreign key
SELECT 
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE tp.name = 'ActivityTypes' AND cp.name = 'MainActivityId';
```

## Important Notes

- **Backup your database** before running migrations
- If you have existing ActivityTypes, they will be assigned to the first MainActivity (by ID)
- After migration, you should update existing ActivityTypes to have the correct MainActivityId
- The migration uses `ReferentialAction.Restrict`, meaning you cannot delete a MainActivity if it has ActivityTypes

## Troubleshooting

If you get an error about existing data:
1. Make sure you have at least one MainActivity in the database
2. If not, create one first
3. Then run the migration again

If migration still fails:
- Check the error message
- You may need to manually set MainActivityId for existing records first
- Then make the column NOT NULL

