# Migration Run Instructions

## ⚠️ Important: Stop the API First!

The API is currently running (process 20412), which is locking the files. You need to:

1. **Stop the running API** (press Ctrl+C in the terminal where it's running, or stop it from Visual Studio)
2. Then run the migration

## Steps to Run Migration

### Option 1: Using Package Manager Console (Recommended)

1. Open Visual Studio
2. **Stop the running API** if it's running
3. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
4. Set the **Default project** to `TES_Learning_App.Infrastructure`
5. Run:
```powershell
Update-Database
```

### Option 2: Using Command Line

1. **Stop the running API** first
2. Open PowerShell or Command Prompt
3. Navigate to backend folder:
```cmd
cd "C:\Users\TFF_DEAD\Desktop\New folder\Trilingo_Backend"
```

4. Run:
```cmd
dotnet ef database update --project "TES_Learning_App.Infrastructure" --startup-project "TES_Learning_App.API"
```

### Option 3: Manual SQL (Fastest - If migration tool doesn't work)

If the migration tool still doesn't work, you can run this SQL directly in SQL Server Management Studio:

```sql
-- Step 1: Add column as nullable
ALTER TABLE ActivityTypes ADD MainActivityId INT NULL;

-- Step 2: Set default for existing records (if any)
UPDATE ActivityTypes 
SET MainActivityId = (SELECT TOP 1 Id FROM MainActivities ORDER BY Id)
WHERE MainActivityId IS NULL;

-- Step 3: Make it NOT NULL
ALTER TABLE ActivityTypes ALTER COLUMN MainActivityId INT NOT NULL;

-- Step 4: Create index
CREATE INDEX IX_ActivityTypes_MainActivityId ON ActivityTypes(MainActivityId);

-- Step 5: Add foreign key
ALTER TABLE ActivityTypes
ADD CONSTRAINT FK_ActivityTypes_MainActivities_MainActivityId
FOREIGN KEY (MainActivityId) REFERENCES MainActivities(Id);
```

## After Migration

Once the migration is complete:
1. Restart your API
2. Try creating an Activity Type again
3. It should work now!

## Verify Migration

To verify the migration worked, run this SQL:

```sql
-- Check if column exists
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ActivityTypes' AND COLUMN_NAME = 'MainActivityId';
```

You should see `MainActivityId` with `int` type and `NO` for nullable.

