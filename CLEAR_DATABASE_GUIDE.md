# Database Clear Guide - RDS SQL Server

இந்த guide-ல் உங்கள் RDS database-ல் உள்ள data-ஐ clear செய்ய எப்படி என்பதை step-by-step-ஆக பார்க்கலாம்.

## ⚠️ IMPORTANT WARNING

**இந்த script-ஐ run செய்வதற்கு முன்:**
- Database-க்கு backup எடுத்துக்கொள்ளுங்கள்
- Production environment-ல் இருந்தால் extra careful-ஆக இருங்கள்
- இந்த script **ALL DATA-ஐ DELETE** செய்யும்

---

## 📋 Prerequisites

1. **SQL Server Management Studio (SSMS)** installed
2. **RDS Database Connection Details:**
   - Server: `trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433`
   - Database: `Trilingo_Learning_Db`
   - Username: `admin`
   - Password: `Lachchu_16`

---

## 🔧 Step 1: Connect to RDS Database via SSMS

1. **SSMS open செய்யுங்கள்**

2. **Connect to Server dialog-ல்:**
   - **Server name:** `trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433`
   - **Authentication:** SQL Server Authentication
   - **Login:** `admin`
   - **Password:** `Lachchu_16`
   - **Connect** button click செய்யுங்கள்

3. **Connection successful-ஆனால்**, Object Explorer-ல் database-ஐ expand செய்யுங்கள்

---

## 🗑️ Step 2: Run the Clear Database Script

### Option A: Using SSMS Query Window

1. **New Query** button click செய்யுங்கள் (Ctrl+N)

2. **`CLEAR_DATABASE.sql` file-ஐ open செய்யுங்கள்**

3. **Script-ஐ copy செய்து query window-ல் paste செய்யுங்கள்**

4. **Execute** button click செய்யுங்கள் (F5)

5. **Messages tab-ல் progress-ஐ check செய்யுங்கள்**

### Option B: Using SQL File Directly

1. SSMS-ல் **File → Open → File** (Ctrl+O)

2. **`CLEAR_DATABASE.sql` file-ஐ select செய்யுங்கள்**

3. **Execute** button click செய்யுங்கள் (F5)

---

## ✅ Step 3: Verify Data is Cleared

Script run ஆன பிறகு, verification queries-ஐ run செய்யலாம்:

```sql
-- Check row counts in all tables
SELECT 'Activities' AS TableName, COUNT(*) AS RowCount FROM [Activities]
UNION ALL
SELECT 'ActivityTypes', COUNT(*) FROM [ActivityTypes]
UNION ALL
SELECT 'MainActivities', COUNT(*) FROM [MainActivities]
UNION ALL
SELECT 'Stages', COUNT(*) FROM [Stages]
UNION ALL
SELECT 'Levels', COUNT(*) FROM [Levels]
UNION ALL
SELECT 'Languages', COUNT(*) FROM [Languages]
UNION ALL
SELECT 'Exercises', COUNT(*) FROM [Exercises]
UNION ALL
SELECT 'StudentProgresses', COUNT(*) FROM [StudentProgresses];
```

**Expected Result:** All tables should show `RowCount = 0`

---

## 🎯 Step 4: Add Data Using Admin Panel

Database clear ஆன பிறகு, admin panel-ல் data add செய்யலாம்:

### Admin Panel URL:
**https://d3v81eez8ecmto.cloudfront.net**

### Data Entry Order:

1. **Languages** (if needed)
   - Admin panel-ல் language add செய்யலாம்

2. **Main Activities**
   - Main Activity page-ல் போய் Main Activities add செய்யுங்கள்
   - Example: "Listening", "Speaking", "Reading", "Writing"

3. **Activity Types**
   - Activity Type page-ல் போய் Activity Types add செய்யுங்கள்
   - **Important:** Each Activity Type-க்கு Main Activity select செய்ய வேண்டும்
   - Example: "Flashcards" → Main Activity: "Listening"

4. **Levels** (if needed)
   - Level page-ல் போய் Levels add செய்யலாம்

5. **Stages** (if needed)
   - Stage page-ல் போய் Stages add செய்யலாம்

6. **Activities**
   - Activity page-ல் போய் Activities add செய்யலாம்
   - Main Activity மற்றும் Activity Type select செய்ய வேண்டும்

---

## 🔄 Alternative: Reset Identity Columns (Optional)

ID counters-ஐ reset செய்ய, script-ல் uncomment செய்யலாம்:

```sql
DBCC CHECKIDENT ('[Activities]', RESEED, 0);
DBCC CHECKIDENT ('[ActivityTypes]', RESEED, 0);
DBCC CHECKIDENT ('[MainActivities]', RESEED, 0);
-- ... etc
```

இதனால், new records 1-லிருந்து start ஆகும்.

---

## 🛠️ Troubleshooting

### Connection Issues

**Problem:** Cannot connect to RDS server

**Solutions:**
- RDS security group-ல் your IP-ஐ allow செய்து பாருங்கள்
- Port 1433 open-ஆக இருக்கிறதா check செய்யுங்கள்
- Network connectivity check செய்யுங்கள்

### Foreign Key Constraint Errors

**Problem:** "The DELETE statement conflicted with the REFERENCE constraint"

**Solution:**
- Script-ல் tables correct order-ல் delete ஆகின்றன
- Error வந்தால், specific table-ஐ manually delete செய்யலாம்
- Or, foreign key constraints-ஐ temporarily disable செய்யலாம்:

```sql
-- Disable all foreign keys (use with caution)
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
```

### Transaction Rollback

**Problem:** Script failed and rolled back

**Solution:**
- Error message-ஐ check செய்யுங்கள்
- Specific table-ல் issue இருக்கலாம்
- Manual-ஆக delete செய்யலாம்

---

## 📝 Notes

- **Roles table** delete ஆகாது (commented out)
- **Users, Admins, Students** data-உம் delete ஆகும்
- Identity columns reset optional-ஆக இருக்கிறது
- Script transaction-ஆக run ஆகிறது, error வந்தால் rollback ஆகும்

---

## ✅ Checklist

Before running the script:
- [ ] Database backup taken
- [ ] SSMS connected to RDS
- [ ] Correct database selected (`Trilingo_Learning_Db`)
- [ ] Script reviewed and understood

After running the script:
- [ ] Verification queries run
- [ ] All tables empty confirmed
- [ ] Admin panel accessible
- [ ] Ready to add new data

---

## 🆘 Support

Issues வந்தால்:
1. Error message-ஐ copy செய்யுங்கள்
2. Which step-ல் error வந்தது என்பதை note செய்யுங்கள்
3. Database connection status check செய்யுங்கள்

---

**Good luck! 🚀**

