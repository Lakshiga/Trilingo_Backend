# 🚀 Quick Start - Backend Deployment

## ⚡ 5-Minute Setup

### 1. Add GitHub Secrets (Required)

Go to: **Repository → Settings → Secrets and variables → Actions**

Add these 3 secrets:

| Secret Name | Description | Example |
|------------|-------------|---------|
| `EC2_HOST` | EC2 public IP or domain | `ec2-12-34-56-78.compute-1.amazonaws.com` |
| `EC2_USER` | SSH username | `ubuntu` or `ec2-user` |
| `EC2_SSH_PRIVATE_KEY` | Full SSH private key | `-----BEGIN RSA PRIVATE KEY-----...` |

### 2. Generate SSH Key (If Needed)

```bash
# Generate key pair
ssh-keygen -t rsa -b 4096 -C "github-actions" -f ~/.ssh/github_actions

# Copy private key to GitHub Secret (EC2_SSH_PRIVATE_KEY)
cat ~/.ssh/github_actions

# Add public key to EC2
ssh-copy-id -i ~/.ssh/github_actions.pub $EC2_USER@$EC2_HOST
```

### 3. Install .NET on EC2

```bash
ssh $EC2_USER@$EC2_HOST
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0 --runtime aspnetcore
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```

### 4. Test Deployment

1. Make a small change to your code
2. Commit and push to `master` or `main`
3. Check **Actions** tab in GitHub
4. Watch it deploy! 🎉

## ✅ That's It!

Your backend will now auto-deploy on every push to `master`/`main`.

## 🔍 Troubleshooting

**Workflow fails?** Check the Actions logs.

**Service won't start?** SSH to EC2 and run:
```bash
sudo journalctl -u trilingo-backend -n 50
```

**Need help?** See [DEPLOYMENT_SETUP.md](./DEPLOYMENT_SETUP.md) for detailed guide.

