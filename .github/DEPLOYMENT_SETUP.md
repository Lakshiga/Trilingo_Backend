# Backend Deployment Setup Guide

This guide will help you set up automated deployment for the Trilingo Learning App Backend using GitHub Actions.

## ⚠️ Important: Workflow File Location

**If your backend is in a separate repository:**
- The workflow file should be at: `.github/workflows/deploy.yml` (repository root)

**If your backend is in a monorepo (subdirectory):**
- Move the workflow file from `Trilingo_Learning_App_Backend/.github/workflows/deploy.yml` 
- To: `.github/workflows/deploy-backend.yml` (repository root)
- The workflow will automatically detect the backend subdirectory

## Prerequisites

1. **AWS EC2 Instance** - Your backend should be running on an EC2 instance
2. **SSH Access** - You need SSH access to your EC2 instance
3. **GitHub Repository** - Your backend code should be in a GitHub repository
4. **.NET 9.0 Runtime** - Installed on your EC2 instance

## Step 1: Prepare Your EC2 Instance

### Install .NET 9.0 Runtime on EC2

SSH into your EC2 instance and run:

```bash
# For Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0 --runtime aspnetcore

# Add to PATH (add to ~/.bashrc or ~/.profile)
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools
```

### Create Application Directory

```bash
sudo mkdir -p /var/www/trilingo-backend
sudo chown -R $USER:$USER /var/www/trilingo-backend
```

### Ensure Your EC2 Security Group Allows:
- Port 5166 (or your backend port) from your application sources
- SSH access (port 22) from GitHub Actions IP ranges (or use a bastion host)

## Step 2: Set Up GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions → New repository secret

Add the following secrets:

### Required Secrets:

1. **EC2_HOST**
   - Value: Your EC2 instance public IP or domain name
   - Example: `ec2-12-34-56-78.compute-1.amazonaws.com` or `api.yourdomain.com`

2. **EC2_USER**
   - Value: Your EC2 SSH username
   - Example: `ubuntu` (for Ubuntu), `ec2-user` (for Amazon Linux), `admin` (for Debian)

3. **EC2_SSH_PRIVATE_KEY**
   - Value: Your private SSH key content (the entire key including `-----BEGIN RSA PRIVATE KEY-----` and `-----END RSA PRIVATE KEY-----`)
   - How to get it:
     ```bash
     # If you don't have a key pair, create one:
     ssh-keygen -t rsa -b 4096 -C "github-actions"
     
     # Copy the private key content
     cat ~/.ssh/id_rsa
     ```
   - **Important**: Add the corresponding public key to your EC2 instance:
     ```bash
     # On your local machine, copy public key to EC2
     ssh-copy-id -i ~/.ssh/id_rsa.pub $EC2_USER@$EC2_HOST
     ```

### Optional Secrets (if using same AWS credentials as frontend):

4. **AWS_ACCESS_KEY** (if needed for other AWS operations)
5. **AWS_SECRET** (if needed for other AWS operations)
6. **AWS_REGION** (if needed for other AWS operations)

## Step 3: Repository Structure

The workflow automatically detects if your backend is:
- **In a separate repository** (backend files at root)
- **In a monorepo** (backend in `Trilingo_Learning_App_Backend/` subdirectory)

The workflow will work correctly in both scenarios.

## Step 4: Configure Your Backend

### Ensure appsettings.Aws.json is Protected

The deployment script automatically preserves `appsettings.Aws.json` to prevent overwriting your AWS configuration. Make sure this file exists on your EC2 instance with the correct settings.

### First-Time Manual Deployment (Optional)

If you want to do a manual first deployment to set up the initial state:

```bash
# On your local machine
cd Trilingo_Learning_App_Backend
dotnet publish TES_Learning_App.API/TES_Learning_App.API.csproj -c Release -o ./publish

# Copy to EC2
scp -r publish/* $EC2_USER@$EC2_HOST:/var/www/trilingo-backend/

# SSH into EC2 and set up the service
ssh $EC2_USER@$EC2_HOST
cd /var/www/trilingo-backend
# Create appsettings.Aws.json with your configuration
# Start the service manually to test
```

## Step 5: Test the Deployment

1. Make a small change to your backend code
2. Commit and push to the `master` or `main` branch
3. Go to your GitHub repository → Actions tab
4. Watch the deployment workflow run
5. Check if your backend is running:
   ```bash
   ssh $EC2_USER@$EC2_HOST
   sudo systemctl status trilingo-backend
   ```

## How It Works

1. **On Push**: When you push to `master` or `main`, GitHub Actions triggers
2. **Build**: The workflow builds your .NET application
3. **Package**: Creates a deployment package (tar.gz)
4. **Deploy**: 
   - Stops the running service
   - Creates a backup of existing files
   - Preserves critical files (wwwroot/uploads, appsettings.Aws.json)
   - Deploys new files
   - Restarts the service
5. **Verify**: Checks if the service started successfully

## Protected Files and Directories

The following are **automatically preserved** during deployment:

- `wwwroot/uploads/` - User uploaded files (profiles, images, etc.)
- `appsettings.Aws.json` - AWS configuration
- `appsettings.Production.json` - Production configuration (if exists)

## Troubleshooting

### Service Fails to Start

Check the logs:
```bash
ssh $EC2_USER@$EC2_HOST
sudo journalctl -u trilingo-backend -n 50 -f
```

### Permission Issues

Ensure the EC2 user has proper permissions:
```bash
sudo chown -R $EC2_USER:$EC2_USER /var/www/trilingo-backend
```

### Connection Issues

Test SSH connection:
```bash
ssh -i ~/.ssh/id_rsa $EC2_USER@$EC2_HOST
```

### .NET Not Found

Ensure .NET is in PATH:
```bash
which dotnet
# If not found, add to PATH in ~/.bashrc
```

## Rollback

If deployment fails, you can rollback using backups:

```bash
ssh $EC2_USER@$EC2_HOST
cd /var/www/trilingo-backend-backup
ls -la  # Find the latest backup
sudo systemctl stop trilingo-backend
sudo rm -rf /var/www/trilingo-backend/*
sudo cp -r /var/www/trilingo-backend-backup/[BACKUP_DATE]/* /var/www/trilingo-backend/
sudo systemctl start trilingo-backend
```

## Security Notes

- Never commit SSH private keys to the repository
- Use GitHub Secrets for all sensitive information
- Regularly rotate SSH keys
- Use IAM roles on EC2 when possible instead of access keys
- Keep your EC2 instance and .NET runtime updated

## Support

If you encounter issues:
1. Check GitHub Actions logs
2. Check EC2 service logs: `sudo journalctl -u trilingo-backend`
3. Verify all secrets are set correctly
4. Ensure EC2 security group allows necessary ports

