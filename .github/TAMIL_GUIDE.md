# 🚀 GitHub Setup Guide - Step by Step (தமிழ்/English)

## 📋 Step-by-Step Instructions

### Step 1: Workflow File Location Check ✅

**Important:** GitHub Actions workflow file must be at the **repository root**, not inside the backend folder.

**Current location:** `Trilingo_Learning_App_Backend/.github/workflows/deploy.yml`

**Required location:** `.github/workflows/deploy.yml` (at repository root)

#### Action Required:
1. Go to your GitHub repository: https://github.com/Lakshiga/Trilingo_Backend
2. Check if `.github` folder exists at the root level
3. If not, create it:
   - Click "Add file" → "Create new file"
   - Type: `.github/workflows/deploy.yml`
   - Copy the content from the workflow file we created
   - Commit the file

### Step 2: Add GitHub Secrets 🔐

Go to: **Settings → Secrets and variables → Actions → New repository secret**

Add these 3 secrets one by one:

#### Secret 1: `EC2_HOST`
- **Name:** `EC2_HOST`
- **Value:** Your EC2 instance public IP or domain
- **Example:** `ec2-12-34-56-78.compute-1.amazonaws.com` or `api.yourdomain.com`
- **How to find:** Check your AWS EC2 console → Instances → Public IPv4 address

#### Secret 2: `EC2_USER`
- **Name:** `EC2_USER`
- **Value:** Your EC2 SSH username
- **Common values:**
  - `ubuntu` (for Ubuntu instances)
  - `ec2-user` (for Amazon Linux)
  - `admin` (for Debian)
- **How to find:** Check your EC2 instance details in AWS console

#### Secret 3: `EC2_SSH_PRIVATE_KEY`
- **Name:** `EC2_SSH_PRIVATE_KEY`
- **Value:** Your complete SSH private key
- **How to get:**

```bash
# Option 1: If you already have a key pair
cat ~/.ssh/id_rsa
# Copy the ENTIRE output including:
# -----BEGIN RSA PRIVATE KEY-----
# ... (all the content) ...
# -----END RSA PRIVATE KEY-----

# Option 2: Create a new key pair for GitHub Actions
ssh-keygen -t rsa -b 4096 -C "github-actions" -f ~/.ssh/github_actions
cat ~/.ssh/github_actions
# Copy the entire output to GitHub Secret

# Then add the public key to your EC2 instance:
ssh-copy-id -i ~/.ssh/github_actions.pub ubuntu@YOUR_EC2_IP
```

**Important:** 
- Copy the ENTIRE key including BEGIN and END lines
- No extra spaces or line breaks
- This is your private key - keep it secure!

### Step 3: Verify Workflow File is Correct 📝

1. Go to your repository: https://github.com/Lakshiga/Trilingo_Backend
2. Navigate to: `.github/workflows/deploy.yml`
3. Verify the file exists and has the deployment workflow content
4. If the file is in `Trilingo_Learning_App_Backend/.github/workflows/`, you need to move it to the root

### Step 4: Prepare EC2 Instance 🖥️

SSH into your EC2 instance and run:

```bash
# Install .NET 9.0 Runtime
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0 --runtime aspnetcore

# Add to PATH (permanent)
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.bashrc
source ~/.bashrc

# Verify installation
dotnet --version
# Should show: 9.0.x

# Create application directory
sudo mkdir -p /var/www/trilingo-backend
sudo chown -R $USER:$USER /var/www/trilingo-backend
```

### Step 5: Test the Deployment 🧪

1. **Make a small change** to your backend code (e.g., add a comment)
2. **Commit and push** to `master` branch:
   ```bash
   git add .
   git commit -m "Test automated deployment"
   git push origin master
   ```
3. **Go to GitHub Actions tab:**
   - Visit: https://github.com/Lakshiga/Trilingo_Backend/actions
   - You should see a workflow run starting
   - Click on it to see the progress
4. **Monitor the deployment:**
   - Green checkmark ✅ = Success
   - Red X ❌ = Check the logs for errors

### Step 6: Verify Deployment Success ✅

After deployment completes:

```bash
# SSH into your EC2 instance
ssh ubuntu@YOUR_EC2_IP

# Check if service is running
sudo systemctl status trilingo-backend

# Check service logs
sudo journalctl -u trilingo-backend -n 50 -f
```

## 🔍 Troubleshooting

### Problem: Workflow not triggering
**Solution:** 
- Check if workflow file is at `.github/workflows/deploy.yml` (root level)
- Ensure you're pushing to `master` or `main` branch
- Check GitHub Actions tab for any errors

### Problem: SSH connection fails
**Solution:**
- Verify `EC2_HOST` secret is correct
- Verify `EC2_USER` secret matches your EC2 username
- Verify `EC2_SSH_PRIVATE_KEY` includes BEGIN/END lines
- Test SSH manually: `ssh -i ~/.ssh/id_rsa ubuntu@YOUR_EC2_IP`

### Problem: Service fails to start
**Solution:**
```bash
# Check logs
sudo journalctl -u trilingo-backend -n 100

# Check if .NET is installed
dotnet --version

# Check permissions
sudo chown -R $USER:$USER /var/www/trilingo-backend
```

### Problem: Files getting overwritten
**Solution:** The workflow automatically preserves:
- `wwwroot/uploads/` - User uploads
- `appsettings.Aws.json` - AWS config
- Backups are created in `/var/www/trilingo-backend-backup/`

## 📝 Quick Checklist

- [ ] Workflow file at `.github/workflows/deploy.yml` (root level)
- [ ] `EC2_HOST` secret added
- [ ] `EC2_USER` secret added
- [ ] `EC2_SSH_PRIVATE_KEY` secret added (complete key)
- [ ] .NET 9.0 installed on EC2
- [ ] Application directory created: `/var/www/trilingo-backend`
- [ ] Test push to `master` branch
- [ ] Check GitHub Actions tab for workflow run
- [ ] Verify service is running on EC2

## 🎉 Success!

Once everything is set up, every time you push to `master` branch, your backend will automatically deploy to AWS EC2!

## 📞 Need Help?

If you encounter issues:
1. Check GitHub Actions logs (Actions tab)
2. Check EC2 service logs: `sudo journalctl -u trilingo-backend`
3. Verify all secrets are correct
4. Ensure EC2 security group allows SSH (port 22) and your app port (5166)

