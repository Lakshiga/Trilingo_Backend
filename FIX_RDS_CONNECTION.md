# Fix RDS Connection Timeout Issue

## 🔴 Problem
```
Error: The wait operation timed out
Cannot connect to RDS database from SSMS
```

## ✅ Solution: Update Security Group

RDS security group-ல் உங்கள் IP address-ஐ allow செய்ய வேண்டும்.

---

## 📋 Step-by-Step Fix

### Step 1: Find Your Public IP Address

1. Browser-ல் போய்: **https://whatismyipaddress.com/**
2. **Your Public IP Address**-ஐ copy செய்யுங்கள்
   - Example: `103.45.67.89`

**OR**

Command Prompt-ல்:
```cmd
curl ifconfig.me
```

---

### Step 2: Update RDS Security Group

1. **AWS Console**-ல் login செய்யுங்கள்
   - https://console.aws.amazon.com

2. **RDS Service**-க்கு போய்:
   - Search bar-ல் "RDS" type செய்யுங்கள்
   - **RDS** service-ஐ select செய்யுங்கள்

3. **Your Database Instance**-ஐ select செய்யுங்கள்:
   - `trilingo-database` (or your instance name)

4. **Connectivity & security** tab-ல்:
   - **VPC security groups** section-ல் security groups-ஐ பாருங்கள்
   - Active security groups:
     - `rds-ec2-1 (sg-0f44f09a2c2a0ca76)`
     - `launch-wizard-1 (sg-087335ee8c73572c9)`
     - `default (sg-0152e5223bdaffac8)`
     - `ec2-rds-1 (sg-0032b3c9443edf9e9)`

5. **Security Group-ஐ click செய்யுங்கள்:**
   - `rds-ec2-1` security group-ஐ click செய்யுங்கள்
   - (அல்லது primary security group-ஐ select செய்யுங்கள்)

6. **Inbound Rules** tab-ல்:
   - **Edit inbound rules** button click செய்யுங்கள்

7. **Add Rule** click செய்யுங்கள்:
   - **Type:** `MSSQL` (or `Custom TCP`)
   - **Port:** `1433`
   - **Source:** `My IP` (automatic-ஆக your IP fill ஆகும்)
     - OR manually: `103.45.67.89/32` (your IP with /32)
   - **Description:** `SSMS Access from My Computer`
   - **Save rules** click செய்யுங்கள்

8. **All Security Groups-க்கும் repeat செய்யுங்கள்:**
   - `rds-ec2-1`
   - `ec2-rds-1`
   - (Other security groups-க்கும் if needed)

---

### Step 3: Wait and Test Connection

1. **2-3 minutes wait** செய்யுங்கள் (security group changes apply ஆக time எடுக்கும்)

2. **SSMS-ல் connect செய்யுங்கள்:**
   ```
   Server: trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433
   Authentication: SQL Server Authentication
   Login: admin
   Password: Lachchu_16
   ```

---

## 🔧 Alternative Solutions

### Option 1: Allow All IPs (Temporary - Not Recommended for Production)

Security group-ல்:
- **Source:** `0.0.0.0/0` (All IPs)
- **Warning:** Security risk - only for testing!

### Option 2: Use AWS Systems Manager Session Manager (More Secure)

If you have EC2 instance in same VPC:
- EC2 instance-லிருந்து RDS-க்கு connect செய்யலாம்
- More secure method

### Option 3: Use AWS CloudShell

1. AWS Console → CloudShell
2. SQL Server client install செய்யலாம்
3. Connect from CloudShell

---

## 🛠️ Troubleshooting

### Still Can't Connect?

1. **Check RDS Status:**
   - RDS Console → Your Database
   - Status should be **Available**
   - If **Stopped**, start it

2. **Verify Security Group:**
   - Inbound rules-ல் port 1433 allow ஆகிறதா?
   - Your IP correct-ஆக add ஆகிறதா?

3. **Check Network:**
   - Firewall/antivirus port 1433-ஐ block செய்கிறதா?
   - Corporate network-ல் இருந்தால், IT team-ஐ contact செய்யுங்கள்

4. **Test Connection from Different Network:**
   - Mobile hotspot use செய்து test செய்யுங்கள்
   - Different IP-லிருந்து connect ஆகிறதா check செய்யுங்கள்

5. **Verify Connection String:**
   ```
   Server: trilingo-database.cxss80scuxgx.ap-southeast-1.rds.amazonaws.com,1433
   ```
   - Port `1433` explicitly specify செய்ய வேண்டும்
   - Comma (`,`) important!

---

## 📝 Quick Security Group Update (AWS CLI)

If you have AWS CLI installed:

```bash
# Get your public IP
MY_IP=$(curl -s ifconfig.me)

# Add rule to security group
aws ec2 authorize-security-group-ingress \
    --group-id sg-0f44f09a2c2a0ca76 \
    --protocol tcp \
    --port 1433 \
    --cidr $MY_IP/32 \
    --region ap-southeast-1
```

---

## ✅ Verification Checklist

- [ ] Public IP address found
- [ ] Security group inbound rule added (port 1433)
- [ ] 2-3 minutes waited for changes to apply
- [ ] SSMS connection tested
- [ ] Connection successful

---

## 🆘 Still Having Issues?

1. **AWS Support** contact செய்யலாம்
2. **RDS Logs** check செய்யலாம்:
   - RDS Console → Your Database → Logs & events
3. **Network ACLs** check செய்யலாம் (VPC level)

---

**Most Common Fix:** Security group-ல் your IP-ஐ allow செய்தால் problem solve ஆகும்! 🎯

