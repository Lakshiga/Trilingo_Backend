# Debug Service Crash - Step by Step

## Issue
Service is crashing with core-dump: `Result: core-dump, signal=ABRT`

## Step 1: Check Service Logs (Most Important!)

```bash
# View recent logs (last 100 lines)
sudo journalctl -u trilingo-backend -n 100 --no-pager

# View logs in real-time
sudo journalctl -u trilingo-backend -f

# View logs with timestamps
sudo journalctl -u trilingo-backend --since "10 minutes ago" --no-pager
```

**Look for:**
- Error messages
- Stack traces
- Database connection errors
- Missing file errors

## Step 2: Check if Environment Variable is Set

The service needs `DATABASE_CONNECTION_STRING` environment variable.

```bash
# Check current service file
sudo cat /etc/systemd/system/trilingo-backend.service

# Check if environment variable is in service file
sudo grep -i "DATABASE_CONNECTION_STRING" /etc/systemd/system/trilingo-backend.service
```

## Step 3: Test Running Application Manually

Stop the service and test manually:

```bash
# Stop the service
sudo systemctl stop trilingo-backend

# Navigate to app directory
cd /var/www/trilingo-backend

# Set environment variable
export DATABASE_CONNECTION_STRING="Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"

# Try running manually
dotnet TES_Learning_App.API.dll
```

**If it works manually**, the issue is with the service configuration.
**If it crashes**, check the error message.

## Step 4: Fix Service File

If environment variable is missing, update the service file:

```bash
# Edit service file
sudo nano /etc/systemd/system/trilingo-backend.service
```

Add this line in the `[Service]` section (after other Environment lines):

```ini
Environment="DATABASE_CONNECTION_STRING=Server=trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433;Database=Trilingo_Learning_Db;User Id=admin;Password=Lachchu_16;Encrypt=true;TrustServerCertificate=true;Connection Timeout=30;MultipleActiveResultSets=true;"
```

Then reload and restart:

```bash
sudo systemctl daemon-reload
sudo systemctl restart trilingo-backend
sudo systemctl status trilingo-backend
```

## Step 5: Check Application Files

```bash
# Check if DLL exists
ls -la /var/www/trilingo-backend/TES_Learning_App.API.dll

# Check permissions
ls -la /var/www/trilingo-backend/

# Check if .NET is available
which dotnet
dotnet --version

# Check if all dependencies are present
cd /var/www/trilingo-backend
ls -la *.dll | head -20
```

## Step 6: Check for Missing Dependencies

```bash
# Check if appsettings files exist
ls -la /var/www/trilingo-backend/appsettings*.json

# Check if wwwroot exists
ls -la /var/www/trilingo-backend/wwwroot/
```

## Common Issues and Solutions

### Issue 1: Missing DATABASE_CONNECTION_STRING
**Solution**: Add it to service file (see Step 4)

### Issue 2: Database Connection Failed
**Solution**: 
- Verify RDS is accessible
- Check security group allows EC2
- Test connection: `telnet trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com 1433`

### Issue 3: Missing .NET Runtime
**Solution**:
```bash
# Check .NET installation
dotnet --list-runtimes

# Install if missing (for .NET 9.0)
sudo dnf install dotnet-runtime-9.0 -y
```

### Issue 4: Permission Issues
**Solution**:
```bash
# Fix ownership
sudo chown -R ec2-user:ec2-user /var/www/trilingo-backend

# Fix permissions
sudo chmod +x /var/www/trilingo-backend/TES_Learning_App.API.dll
```

### Issue 5: Program.cs Error (from recent changes)
**Solution**: Check if `ToList()` is available (needs `using System.Linq;`)

## Quick Fix Script

Run this to check everything:

```bash
#!/bin/bash
echo "=== Checking Service Status ==="
sudo systemctl status trilingo-backend --no-pager -l

echo -e "\n=== Checking Logs ==="
sudo journalctl -u trilingo-backend -n 50 --no-pager

echo -e "\n=== Checking Service File ==="
sudo cat /etc/systemd/system/trilingo-backend.service

echo -e "\n=== Checking Application Files ==="
ls -la /var/www/trilingo-backend/ | head -20

echo -e "\n=== Checking .NET ==="
which dotnet
dotnet --version

echo -e "\n=== Checking Environment Variable in Service ==="
sudo grep -i "DATABASE_CONNECTION_STRING" /etc/systemd/system/trilingo-backend.service || echo "❌ DATABASE_CONNECTION_STRING not found in service file!"
```

Save as `check-service.sh`, make executable, and run:
```bash
chmod +x check-service.sh
./check-service.sh
```



















