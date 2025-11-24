# Fix Service Crash - Quick Steps

## Step 1: Check Service Logs (See the actual error)

```bash
sudo journalctl -u trilingo-backend -n 100 --no-pager
```

**Copy the error message and share it!**

## Step 2: Check Current Service File

```bash
sudo cat /etc/systemd/system/trilingo-backend.service
```

## Step 3: Fix Service File (Add DATABASE_CONNECTION_STRING)

The service file is missing the DATABASE_CONNECTION_STRING environment variable. Fix it:

```bash
sudo nano /etc/systemd/system/trilingo-backend.service
```

Find the `[Service]` section and add this line after the other `Environment=` lines:

```ini
Environment="DATABASE_CONNECTION_STRING=Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"
```

The service file should look like this:

```ini
[Unit]
Description=Trilingo Learning App Backend API
After=network.target

[Service]
Type=simple
User=ec2-user
WorkingDirectory=/var/www/trilingo-backend
ExecStart=/usr/bin/dotnet /var/www/trilingo-backend/TES_Learning_App.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=trilingo-backend
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment="DATABASE_CONNECTION_STRING=Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"

[Install]
WantedBy=multi-user.target
```

Save: `Ctrl+O`, `Enter`, `Ctrl+X`

## Step 4: Reload and Restart

```bash
sudo systemctl daemon-reload
sudo systemctl restart trilingo-backend
sudo systemctl status trilingo-backend
```

## Step 5: Check Logs Again

```bash
# Watch logs in real-time
sudo journalctl -u trilingo-backend -f
```

You should see:
- ✅ "Applying X pending database migration(s)..."
- ✅ "Database migrations applied successfully!"
- ✅ Application starting

## Step 6: Verify Migrations Applied

```bash
# Check if migrations were applied
sudo journalctl -u trilingo-backend | grep -i migration
```

---

## Alternative: Test Manually First

If you want to test before fixing the service:

```bash
# Stop service
sudo systemctl stop trilingo-backend

# Go to app directory
cd /var/www/trilingo-backend

# Set environment variable
export DATABASE_CONNECTION_STRING="Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"

# Run manually
dotnet TES_Learning_App.API.dll
```

If it works manually, the issue is the service file. Fix it using Step 3 above.

---

## Note About EF Tools

**You don't need to run `dotnet ef` manually!** 

The application automatically applies migrations on startup (we added this code in Program.cs). Just:
1. Fix the service file (add DATABASE_CONNECTION_STRING)
2. Restart the service
3. Migrations will apply automatically



















