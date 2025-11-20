# 🔑 How to Get SSH Private Key for GitHub Secrets

## Method 1: If you already have SSH key pair

```bash
# On your local computer, open terminal/PowerShell
cat ~/.ssh/id_rsa

# Or on Windows:
type C:\Users\ASUS\.ssh\id_rsa

# Copy the ENTIRE output including:
# -----BEGIN RSA PRIVATE KEY-----
# (all the content)
# -----END RSA PRIVATE KEY-----
```

## Method 2: Create new SSH key pair for GitHub Actions

```bash
# Generate new key pair
ssh-keygen -t rsa -b 4096 -C "github-actions" -f ~/.ssh/github_actions

# View the private key (copy this to GitHub Secret)
cat ~/.ssh/github_actions

# Add public key to your EC2 instance
ssh-copy-id -i ~/.ssh/github_actions.pub ubuntu@YOUR_EC2_IP

# Or manually:
cat ~/.ssh/github_actions.pub
# Then SSH to EC2 and add to ~/.ssh/authorized_keys
```

## Method 3: If using AWS EC2 Key Pair

If you downloaded a `.pem` file from AWS:

```bash
# Convert .pem to standard format (if needed)
# The .pem file IS your private key, just copy its content
cat your-key.pem

# Copy the entire content to GitHub Secret
```

## ⚠️ Important Notes:

1. **Copy the ENTIRE key** - including `-----BEGIN` and `-----END` lines
2. **No extra spaces** - paste exactly as shown
3. **Keep it secret** - never share your private key publicly
4. **Test connection** before adding to GitHub:
   ```bash
   ssh -i ~/.ssh/id_rsa ubuntu@YOUR_EC2_IP
   ```

